using MediatR;
using Monitoring_Service.Application.DTOs;
using Monitoring_Service.Application.Interface;
using Monitoring_Service.Domain.Entity;
using System.Text.Json;

namespace Monitoring_Service.Application.HandleRawTestResult.Command
{
    /// <summary>
    /// Handles the raw test result.
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;Monitoring_Service.Application.HandleRawTestResult.Command.HandleRawTestResultCommand, System.Boolean&gt;" />
    public class HandleRawTestResultHandler : IRequestHandler<HandleRawTestResultCommand, bool>
    {
        /// <summary>
        /// The repository
        /// </summary>
        private readonly IRawResultRepository _repository;
        /// <summary>
        /// The laboratory publisher
        /// </summary>
        private readonly ILaboratoryPublisher _laboratoryPublisher;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandleRawTestResultHandler"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="laboratoryPublisher">The laboratory publisher.</param>
        public HandleRawTestResultHandler(
            IRawResultRepository repository,
            ILaboratoryPublisher laboratoryPublisher)
        {
            _repository = repository;
            _laboratoryPublisher = laboratoryPublisher;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<bool> Handle(HandleRawTestResultCommand request, CancellationToken cancellationToken)
        {
            var rawOrderDto = JsonSerializer.Deserialize<RawTestResultDTO>(request.RawJson);
            if (rawOrderDto == null)
            {
                return false;
            }

            var backupEntities = new List<RawTestResult>();

            foreach (var itemDto in rawOrderDto.Results)
            {
                bool exists = await _repository.ExistsAsync(rawOrderDto.TestOrderId, itemDto.TestCode);
                if (exists)
                {
                    continue;
                }

                var entity = new RawTestResult
                {
                    TestOrderId = rawOrderDto.TestOrderId,
                    Instrument = rawOrderDto.Instrument,
                    PerformedDate = rawOrderDto.PerformedDate,
                    TestCode = itemDto.TestCode,
                    Parameter = itemDto.Parameter,
                    ValueNumeric = itemDto.ValueNumeric,
                    ValueText = itemDto.ValueText,
                    Unit = itemDto.Unit,
                    ReferenceRange = itemDto.ReferenceRange,
                    ResultStatus = itemDto.Status,
                    Status = itemDto.Status,
                    PerformedByUserId = null,
                    ReviewedByUserId = null,
                    ReviewedDate = null,
                    CreatedAt = DateTime.UtcNow
                };
                backupEntities.Add(entity);
            }

            if (backupEntities.Any())
            {
                await _repository.AddBackupRangeAsync(backupEntities);
            }

            await _laboratoryPublisher.PublishAsync(rawOrderDto);

            return true;
        }
    }
}