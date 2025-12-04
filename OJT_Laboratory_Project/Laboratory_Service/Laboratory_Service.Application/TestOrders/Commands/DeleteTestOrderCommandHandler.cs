using Laboratory_Service.Application.Interface;
using Laboratory_Service.Domain.Entity;
using MediatR;

namespace Laboratory_Service.Application.TestOrders.Commands
{
    /// <summary>
    /// Handle Delete Test Order Command
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;Laboratory_Service.Application.TestOrders.Commands.DeleteTestOrderCommand, System.Boolean&gt;" />
    public class DeleteTestOrderCommandHandler : IRequestHandler<DeleteTestOrderCommand, bool>
    {
        /// <summary>
        /// The repository
        /// </summary>
        private readonly ITestOrderRepository _repository;
        /// <summary>
        /// The event log service
        /// </summary>
        private readonly IEventLogService _eventLogService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteTestOrderCommandHandler"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="eventLogService">The event log service.</param>
        public DeleteTestOrderCommandHandler(ITestOrderRepository repository, IEventLogService eventLogService)
        {
            _repository = repository;
            _eventLogService = eventLogService;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Test order not found.</exception>
        /// <exception cref="System.InvalidOperationException">Test order already deleted.</exception>
        public async Task<bool> Handle(DeleteTestOrderCommand request, CancellationToken cancellationToken)
        {
            // Use GetByIdForUpdateAsync to avoid SQL issues with includes when we only need to update/delete
            var testOrder = await _repository.GetByIdForUpdateAsync(request.TestOrderId, cancellationToken);
            if (testOrder == null)
                throw new KeyNotFoundException("Test order not found.");

            if (testOrder.IsDeleted)
                throw new InvalidOperationException("Test order already deleted.");

            var deletedBy = request.DeletedBy;
            if (testOrder.Status == "Completed")
            {
                testOrder.IsDeleted = true;
                testOrder.DeletedAt = DateTime.UtcNow;
                testOrder.DeletedBy = deletedBy;
                await _repository.UpdateAsync(testOrder);
            }
            else
            {
                await _repository.DeleteAsync(testOrder);
            }

            var deletedByUsername = request.DeletedBy;
            await _eventLogService.CreateAsync(new EventLog
            {
                EventId = "E_00003",
                Action = "Delete Test Order",
                EventLogMessage = $"Test Order {testOrder.OrderCode} deleted by {deletedBy}",
                OperatorName = deletedByUsername,
                EntityType = "TestOrder",
                EntityId = testOrder.TestOrderId,
                CreatedOn = DateTime.UtcNow
            });


            return true;
        }

    }
}
