using Laboratory_Service.Application.DTOs.Patients;
using MediatR;

namespace Laboratory_Service.Application.Patients.Queries
{
    /// <summary>
    /// create class GetAllPatientsQuery
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;System.Collections.Generic.List&lt;Laboratory_Service.Application.DTOs.Patients.PatientDto&gt;&gt;" />
    public class GetAllPatientsQuery : IRequest<List<PatientDto>>
    {

    }
}
