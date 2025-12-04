using Laboratory_Service.Application.Interface;
using MediatR;

namespace Laboratory_Service.Application.AiReviewForTestOrder.Command
{
    /// <summary>
    /// Create SetAiReviewModeForTestOrderCommandHandler
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;Laboratory_Service.Application.AiReviewForTestOrder.Command.SetAiReviewModeForTestOrderCommand, MediatR.Unit&gt;" />
    public class SetAiReviewModeForTestOrderCommandHandler : IRequestHandler<SetAiReviewModeForTestOrderCommand, Unit>
    {
        /// <summary>
        /// The repository
        /// </summary>
        private readonly ITestOrderRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetAiReviewModeForTestOrderCommandHandler"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public SetAiReviewModeForTestOrderCommandHandler(ITestOrderRepository repository)
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
        /// <exception cref="System.Exception">Test order not found.</exception>
        public async Task<Unit> Handle(SetAiReviewModeForTestOrderCommand request, CancellationToken cancellationToken)
        {
            // Use GetByIdForUpdateAsync to avoid SQL issues with includes when we only need to update IsAiReviewEnabled
            var testOrder = await _repository.GetByIdForUpdateAsync(request.TestOrderId, cancellationToken);
            if (testOrder == null || testOrder.IsDeleted)
                throw new Exception("Test order not found.");

            testOrder.IsAiReviewEnabled = request.Enable;
            await _repository.UpdateAsync(testOrder);

            return Unit.Value;
        }
    }
}
