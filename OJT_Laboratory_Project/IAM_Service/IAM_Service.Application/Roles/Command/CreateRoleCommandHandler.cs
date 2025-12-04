using IAM_Service.Application.DTOs.Roles;
using IAM_Service.Application.Interface.IPrivilege;
using IAM_Service.Application.Interface.IRole;
using IAM_Service.Domain.Entity;
using MediatR;

namespace IAM_Service.Application.Roles.Command
{
    /// <summary>
    /// Handle create role 
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;IAM_Service.Application.Roles.Command.CreateRoleCommand, IAM_Service.Application.DTOs.Roles.RoleDto&gt;" />
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, RoleDto>
    {
        /// <summary>
        /// The role repo
        /// </summary>
        private readonly IRoleCommandRepository _roleRepo;
        /// <summary>
        /// The priv repo
        /// </summary>
        private readonly IPrivilegeRepository _privRepo;
        private readonly IPrivilegeNormalizationService _privilegeNormalizationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateRoleCommandHandler"/> class.
        /// </summary>
        /// <param name="roleRepo">The role repo.</param>
        /// <param name="privRepo">The priv repo.</param>
        public CreateRoleCommandHandler(IRoleCommandRepository roleRepo, IPrivilegeRepository privRepo, IPrivilegeNormalizationService privilegeNormalizationService)
        {
            _roleRepo = roleRepo;
            _privRepo = privRepo;
            _privilegeNormalizationService = privilegeNormalizationService;
        }

        /// <summary>
        /// Handles the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="ct">The ct.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">
        /// Role with code '{dto.Code}' already exists.
        /// or
        /// No valid privileges found for given IDs.
        /// </exception>
        /// <exception cref="System.Exception">Default READ_ONLY privilege not found in database.</exception>
        public async Task<RoleDto> Handle(CreateRoleCommand request, CancellationToken ct)
        {
            var dto = request.Dto;

            // ✅ AC01: Kiểm tra code trùng
            if (await _roleRepo.ExistsByCodeAsync(dto.Code, ct))
                throw new InvalidOperationException($"Role with code '{dto.Code}' already exists.");

            // 🎯 Bước 1: Chuẩn hóa và tự động thêm quyền View/Read nếu cần
            var privilegeIds = _privilegeNormalizationService.Normalize(dto.PrivilegeIds);

            var role = new Role
            {
                Name = dto.Name.Trim(),
                Code = dto.Code.Trim().ToUpperInvariant(),
                Description = dto.Description,
                RolePrivileges = new List<RolePrivilege>()
            };

            // ✅ AC02: Lấy danh sách privileges
            List<Privilege> privileges;

            if (privilegeIds == null || privilegeIds.Count == 0)
            {
                // Nếu không có privilegeIds nào sau khi chuẩn hóa => mặc định READ_ONLY
                var readOnly = await _privRepo.GetByNameAsync("READ_ONLY", ct);

                if (readOnly == null)
                    throw new Exception("Default READ_ONLY privilege not found in database.");

                privileges = new List<Privilege> { readOnly };
            }
            else
            {
                // Lấy các privileges theo ID đã chuẩn hóa
                privileges = await _privRepo.GetPrivilegesByIdsAsync(privilegeIds, ct);

                if (privileges == null || privileges.Count == 0)
                    throw new InvalidOperationException("No valid privileges found for given IDs.");
            }

            // ✅ Gán quan hệ Role – Privilege
            foreach (var p in privileges)
            {
                role.RolePrivileges.Add(new RolePrivilege
                {
                    Role = role,
                    Privilege = p,
                    RoleId = role.RoleId,
                    PrivilegeId = p.PrivilegeId
                });
            }

            await _roleRepo.AddAsync(role, ct);
            await _roleRepo.SaveChangesAsync(ct);

            return new RoleDto
            {
                RoleId = role.RoleId,
                Name = role.Name,
                Code = role.Code,
                Description = role.Description,
                Privileges = privileges.Select(p => p.Name).ToList()
            };
        }
    }
}
