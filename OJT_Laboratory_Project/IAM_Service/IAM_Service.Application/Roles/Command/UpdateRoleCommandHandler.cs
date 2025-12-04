using AutoMapper;
using IAM_Service.Application.DTOs.Roles;
using IAM_Service.Application.Interface.IAuditLogRepository;
using IAM_Service.Application.Interface.IPrivilege;
using IAM_Service.Application.Interface.IRole;
using IAM_Service.Domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IAM_Service.Application.Roles.Command
{
    /// <summary>
    /// Handles the update of a role.
    /// </summary>
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, RoleDto>
    {
        private readonly IRoleCommandRepository _roleCommandRepository;
        private readonly IPrivilegeRepository _privilegeRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateRoleCommandHandler> _logger;
        // 🎯 Thêm Service chuẩn hóa
        private readonly IPrivilegeNormalizationService _privilegeNormalizationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateRoleCommandHandler"/> class.
        /// </summary>
        public UpdateRoleCommandHandler(
            IRoleCommandRepository roleCommandRepository,
            IPrivilegeRepository privilegeRepository,
            IAuditLogRepository auditLogRepository,
            IMapper mapper,
            ILogger<UpdateRoleCommandHandler> logger,
            IPrivilegeNormalizationService privilegeNormalizationService)
        {
            _roleCommandRepository = roleCommandRepository;
            _privilegeRepository = privilegeRepository;
            _auditLogRepository = auditLogRepository;
            _mapper = mapper;
            _logger = logger;
            _privilegeNormalizationService = privilegeNormalizationService;
        }

        /// <summary>
        /// Handles the update role request.
        /// </summary>
        public async Task<RoleDto> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating role with ID {RoleId}", request.RoleId);

            // ✅ 1. Kiểm tra role có tồn tại không
            var existingRole = await _roleCommandRepository.GetByIdWithTrackingAsync(request.RoleId, cancellationToken);
            if (existingRole == null)
            {
                _logger.LogWarning("Role with ID {RoleId} not found", request.RoleId);
                throw new KeyNotFoundException($"Role with ID {request.RoleId} not found.");
            }

            // Sử dụng service để tự động thêm các quyền View/Read nếu quyền Modify được chọn
            var normalizedPrivilegeIds = _privilegeNormalizationService.Normalize(request.Dto.PrivilegeIds);

            //  3. Lấy danh sách Privilege hợp lệ
            var privileges = new List<Privilege>();

            if (normalizedPrivilegeIds != null && normalizedPrivilegeIds.Any())
            {
                privileges = await _privilegeRepository.GetPrivilegesByIdsAsync(normalizedPrivilegeIds, cancellationToken);

                // Vì danh sách đã được chuẩn hóa (loại bỏ trùng lặp), ta chỉ cần kiểm tra xem 
                // có ID nào không tìm thấy trong DB hay không.
                if (privileges.Count != normalizedPrivilegeIds.Count)
                {
                    _logger.LogWarning("One or more privilege IDs are invalid after normalization");
                    throw new KeyNotFoundException("One or more privilege IDs are invalid.");
                }
            }
            // KHÔNG cần kiểm tra mặc định READ_ONLY ở đây vì đây là Update.
            // Nếu danh sách quyền mới rỗng, ta vẫn cho phép gán quyền rỗng hoặc theo logic nghiệp vụ khác.

            //  4. Cập nhật thông tin role
            existingRole.Name = request.Dto.Name.Trim();
            existingRole.Description = request.Dto.Description;
            // Bạn có thể cần xem xét lại việc cho phép cập nhật Role Code (Code), 
            // vì Code thường là định danh duy nhất không nên thay đổi, 
            // nhưng tôi giữ nguyên code của bạn:
            existingRole.Code = request.Dto.Code.Trim().ToUpperInvariant(); // Đảm bảo Code luôn là UPPER

            // ✅ 5. Cập nhật lại danh sách quyền (Xóa cũ, Thêm mới)
            existingRole.RolePrivileges.Clear();
            foreach (var privilege in privileges)
            {
                existingRole.RolePrivileges.Add(new RolePrivilege
                {
                    RoleId = existingRole.RoleId,
                    PrivilegeId = privilege.PrivilegeId
                });
            }

            // ✅ 6. Lưu thay đổi role
            await _roleCommandRepository.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Successfully updated role with ID {RoleId}", request.RoleId);

            // ✅ 7. Ghi log AC03 - chuẩn theo model AuditLog của bạn
            try
            {
                // Danh sách quyền mới
                var privilegeNames = privileges.Any()
                    ? string.Join(", ", privileges.Select(p => p.Name))
                    : "(no privileges)";

                var auditLog = new AuditLog
                {
                    UserEmail = "admin@fpt.com", // ⚠️ TODO: nếu bạn có HttpContext, lấy email từ token
                    EntityName = "Role",
                    Action = "Update",
                    Changes = $"Updated role '{existingRole.Name}' with privileges: {privilegeNames}",
                    Timestamp = DateTime.UtcNow
                };

                await _auditLogRepository.AddAsync(auditLog, cancellationToken);

                _logger.LogInformation("Audit log recorded for updating role: {RoleName}", existingRole.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write audit log for role update (Role ID {RoleId})", request.RoleId);
            }


            // ✅ 8. Map lại DTO để trả về FE
            var roleDto = _mapper.Map<RoleDto>(existingRole);
            roleDto.Privileges = privileges.Select(p => p.Name).ToList();

            return roleDto;
        }
    }
}