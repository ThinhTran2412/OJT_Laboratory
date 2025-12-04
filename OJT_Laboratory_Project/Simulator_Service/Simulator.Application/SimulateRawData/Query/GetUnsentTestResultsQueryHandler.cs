using MediatR;
using Simulator.Application.DTOs;
using Simulator.Application.Interface;

namespace Simulator.Application.SimulateRawData.Query
{
    /// <summary>
    /// Handles the get unsent test results query
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;Simulator.Application.SimulateRawData.Query.GetUnsentTestResultsQuery, System.Collections.Generic.IEnumerable&lt;Simulator.Application.DTOs.RawTestResultDTO&gt;&gt;" />
    public class GetUnsentTestResultsQueryHandler : IRequestHandler<GetUnsentTestResultsQuery, IEnumerable<RawTestResultDTO>>
    {
        /// <summary>
        /// The repository
        /// </summary>
        private readonly IRawTestResultRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetUnsentTestResultsQueryHandler"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public GetUnsentTestResultsQueryHandler(IRawTestResultRepository repository)
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
        public async Task<IEnumerable<RawTestResultDTO>> Handle(GetUnsentTestResultsQuery request, CancellationToken cancellationToken)
        {
            var entities = await _repository.GetUnsentResultsAsync();

            if (entities == null || !entities.Any())
            {
                return Enumerable.Empty<RawTestResultDTO>();
            }
            return entities.ToList();
        }
    }
}