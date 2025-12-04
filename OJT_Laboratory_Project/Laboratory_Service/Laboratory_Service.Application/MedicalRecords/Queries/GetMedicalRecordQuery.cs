using Laboratory_Service.Application.DTOs.MedicalRecords;
using MediatR;

namespace Laboratory_Service.Application.MedicalRecords.Queries
{
    /// <summary>
    /// Create GetAllMedicalRecordsQuery
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;System.Collections.Generic.List&lt;Laboratory_Service.Application.DTOs.MedicalRecords.MedicalRecordViewDto&gt;&gt;" />
    /// <seealso cref="MediatR.IBaseRequest" />
    /// <seealso cref="System.IEquatable&lt;Laboratory_Service.Application.MedicalRecords.Queries.GetAllMedicalRecordsQuery&gt;" />
    public record GetAllMedicalRecordsQuery() : IRequest<List<MedicalRecordViewDto>>;
}
