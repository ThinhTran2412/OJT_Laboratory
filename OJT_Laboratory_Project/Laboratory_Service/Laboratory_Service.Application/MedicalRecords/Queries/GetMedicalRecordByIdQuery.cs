using Laboratory_Service.Application.DTOs.MedicalRecords;
using MediatR;

namespace Laboratory_Service.Application.MedicalRecords.Queries
{
    /// <summary>
    /// Create GetMedicalRecordByIdQuery
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;Laboratory_Service.Application.DTOs.MedicalRecords.MedicalRecordViewDto&gt;" />
    /// <seealso cref="MediatR.IBaseRequest" />
    /// <seealso cref="System.IEquatable&lt;Laboratory_Service.Application.MedicalRecords.Queries.GetMedicalRecordByIdQuery&gt;" />
    public record GetMedicalRecordByIdQuery(int MedicalRecordId) : IRequest<MedicalRecordViewDto>;
}

