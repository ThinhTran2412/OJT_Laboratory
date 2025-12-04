using Laboratory_Service.API.Protos;
using Laboratory_Service.Application.DTOs.Patients;

namespace Laboratory_Service.API.Mappers
{
    /// <summary>
    /// Creates the patient mapper.
    /// </summary>
    public static class PatientMapper
    {
        // Map from PatientDto to gRPC model so API layer no longer needs to expose domain entities directly
        /// <summary>
        /// Converts to grpcmodel.
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <returns></returns>
        public static PatientData ToGrpcModel(this PatientDto dto)
        {
            if (dto == null)
                return null;

            return new PatientData
            {
                PatientId = dto.PatientId,
                IdentifyNumber = dto.IdentifyNumber,
                FullName = dto.FullName,
                // PatientDto.DateOfBirth is non-nullable DateOnly
                DateOfBirth = dto.DateOfBirth.ToString("yyyy-MM-dd"),
                Gender = dto.Gender,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                Address = dto.Address,
                Age = dto.Age,
                CreatedAt = dto.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                // PatientDto does not contain UpdatedAt property in this application DTO; provide empty string
                UpdatedAt = string.Empty,
                CreatedBy = dto.CreatedBy
            };
        }
    }
}