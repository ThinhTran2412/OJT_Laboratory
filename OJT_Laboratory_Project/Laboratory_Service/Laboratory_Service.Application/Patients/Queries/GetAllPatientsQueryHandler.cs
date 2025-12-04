using AutoMapper;
using Laboratory_Service.Application.DTOs.Patients;
using Laboratory_Service.Application.Interface;
using MediatR;

namespace Laboratory_Service.Application.Patients.Queries
{
    /// <summary>
    /// Handle for GetAllPatientsQueryHandler
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;Laboratory_Service.Application.Patients.Queries.GetAllPatientsQuery, System.Collections.Generic.List&lt;Laboratory_Service.Application.DTOs.Patients.PatientDto&gt;&gt;" />
    public class GetAllPatientsQueryHandler : IRequestHandler<GetAllPatientsQuery, List<PatientDto>>
    {
        /// <summary>
        /// The patient repository
        /// </summary>
        private readonly IPatientRepository _patientRepository;
        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllPatientsQueryHandler"/> class.
        /// </summary>
        /// <param name="patientRepository">The patient repository.</param>
        /// <param name="mapper">The mapper.</param>
        public GetAllPatientsQueryHandler(IPatientRepository patientRepository, IMapper mapper)
        {
            _patientRepository = patientRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<List<PatientDto>> Handle(GetAllPatientsQuery request, CancellationToken cancellationToken)
        {
            var patients = await _patientRepository.GetAllAsync();
            return _mapper.Map<List<PatientDto>>(patients);
        }
    }
}
