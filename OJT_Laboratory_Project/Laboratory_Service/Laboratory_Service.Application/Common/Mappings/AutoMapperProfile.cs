using AutoMapper;
using Laboratory_Service.Application.DTOs.Comment;
using Laboratory_Service.Application.DTOs.MedicalRecords;
using Laboratory_Service.Application.DTOs.Patients;
using Laboratory_Service.Application.DTOs.User;
using Laboratory_Service.Domain.Entity;
using DomainComment = Laboratory_Service.Domain.Entity.Comment;

/// <summary>
/// AutoMapper profile for mapping between domain entities and DTOs.
/// </summary>
namespace Laboratory_Service.Application.Common.Mappings
{
    /// <summary>
    /// Configuration profile for AutoMapper.
    ///     </summary>
    public class AutoMapperProfile : Profile
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMapperProfile"/> class.
        ///     </summary>
        public AutoMapperProfile()
        {
            // Patient mappings
            CreateMap<Patient, PatientDto>()
                .ForMember(dest => dest.LastTestDate, opt => opt.Ignore());
            CreateMap<CreatePatientClient, Patient>();

            // Medical Record mappings
            CreateMap<MedicalRecord, MedicalRecordDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.FullName));

            // User Info mappings
            CreateMap<UserInfo, Patient>()
                .ForMember(dest => dest.PatientId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.MedicalRecord, opt => opt.Ignore());

            CreateMap<UserInfo, UserDataDTO>();

            // Comment mappings
            CreateMap<CreateCommentDto, DomainComment>()
                 // Trích xuất phần tử đầu tiên của danh sách để gán cho thuộc tính TestResultId (int?) của Entity
                 .ForMember(dest => dest.TestResultId, opt => opt.MapFrom(src =>
                     src.TestResultId != null && src.TestResultId.Count > 0
                         ? (int?)src.TestResultId[0]
                         : (int?)null))
                 .ForMember(dest => dest.CommentId, opt => opt.Ignore())
                 .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                 .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                 .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                 .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                 .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                 .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                 .ForMember(dest => dest.DeletedDate, opt => opt.Ignore());

            // Ánh xạ 2: DomainComment -> CommentDto (Đã loại bỏ ánh xạ trùng lặp)
            CreateMap<DomainComment, CommentDto>()
                // Giữ lại ánh xạ trực tiếp từ Entity sang DTO nếu cả hai đều là int/int?
                // AutoMapper sẽ tự xử lý ánh xạ này, không cần gọi MapFrom nếu tên trường giống nhau.
                // Tôi chỉ giữ lại các ánh xạ cần thiết/tường minh để tránh lỗi.
                .ForMember(dest => dest.CommentId, opt => opt.MapFrom(src => src.CommentId))
                // Bỏ phần ánh xạ phức tạp và trùng lặp. Chỉ ánh xạ trực tiếp TestResultId nếu nó là int?
                .ForMember(dest => dest.TestResultId, opt => opt.MapFrom(src => src.TestResultId))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.ModifiedBy))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate));
        }
    }
}
