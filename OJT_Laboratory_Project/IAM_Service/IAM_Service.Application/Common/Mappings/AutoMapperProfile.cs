using AutoMapper;
using IAM_Service.Application.DTOs;
using IAM_Service.Application.DTOs.Privileges;
using IAM_Service.Application.DTOs.Roles;
using IAM_Service.Application.DTOs.User;
using IAM_Service.Application.Registers.Command;
using IAM_Service.Application.Users.Command;
using IAM_Service.Domain.Entity;

/// <summary>
/// AutoMapper profile for mapping between domain entities and DTOs.
/// </summary>
namespace IAM_Service.Application.Common.Mappings
{
    /// <summary>
    /// Configuration profile for AutoMapper.
    /// </summary>
    public class AutoMapperProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMapperProfile"/> class.
        /// </summary>
        public AutoMapperProfile()
        {
            // Example 
            // CreateMap<User, UserDTO>().ReverseMap();

            CreateMap<CreateUserCommand, User>();
            CreateMap<User, UserDetailDto>();
            CreateMap<RegistersAccountCommand, User>();
            CreateMap<AuditLog, AuditLogDto>().ReverseMap();
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, UserProfileDTO>();
            CreateMap<Privilege, PrivilegeDto>();
            CreateMap<Role, RoleDto>().ReverseMap();
            CreateMap<Role, UpdateRoleDto>().ReverseMap();
        }
    }
}
