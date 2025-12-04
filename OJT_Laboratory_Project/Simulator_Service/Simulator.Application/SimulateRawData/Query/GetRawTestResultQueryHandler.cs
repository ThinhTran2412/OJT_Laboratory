using MediatR;
using Simulator.Application.DTOs;
using Simulator.Application.Interface;

namespace Simulator.Application.SimulateRawData.Query
{
    /// <summary>
    /// Handles the get raw test result query
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;Simulator.Application.SimulateRawData.Query.GetRawTestResultQuery, Simulator.Application.DTOs.RawTestResultDTO&gt;" />
    public class GetRawTestResultQueryHandler : IRequestHandler<GetRawTestResultQuery, RawTestResultDTO?>
    {
        /// <summary>
        /// The repository
        /// </summary>
        private readonly IRawTestResultRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetRawTestResultQueryHandler"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public GetRawTestResultQueryHandler(IRawTestResultRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<RawTestResultDTO?> Handle(GetRawTestResultQuery request, CancellationToken cancellationToken)
        {
            var entities = await _repository.GetResultsByOrderIdAsync(request.TestOrderId);

            if (entities == null || !entities.Any())
            {
                return null;
            }

            var firstItem = entities.First();

            var dto = new RawTestResultDTO
            {
                TestOrderId = firstItem.TestOrderId,
                Instrument = firstItem.Instrument,
                PerformedDate = firstItem.PerformedDate,
                Results = entities.Select(e => new RawResultItemDTO
                {
                    TestCode = e.TestCode,
                    Parameter = e.Parameter,
                    ValueNumeric = e.ValueNumeric,
                    ValueText = e.ValueText,
                    Unit = e.Unit,
                    ReferenceRange = e.ReferenceRange,
                    Status = e.Status
                }).ToList()
            };

            return dto;
        }
    }
}
