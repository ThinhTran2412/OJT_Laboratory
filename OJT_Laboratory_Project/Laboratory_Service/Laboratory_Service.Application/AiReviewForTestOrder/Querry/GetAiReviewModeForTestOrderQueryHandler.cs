using Laboratory_Service.Application.Interface;
using MediatR;

namespace Laboratory_Service.Application.AiReviewForTestOrder.Querry
{
    /// <summary>
    /// Create GetAiReviewModeForTestOrderQueryHandler
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;Laboratory_Service.Application.AiReviewForTestOrder.Querry.GetAiReviewModeForTestOrderQuery, System.Boolean&gt;" />
    public class GetAiReviewModeForTestOrderQueryHandler : IRequestHandler<GetAiReviewModeForTestOrderQuery, bool>
    {
        /// <summary>
        /// The repository
        /// </summary>
        private readonly ITestOrderRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAiReviewModeForTestOrderQueryHandler"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public GetAiReviewModeForTestOrderQueryHandler(ITestOrderRepository repository)
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
        public async Task<bool> Handle(GetAiReviewModeForTestOrderQuery request, CancellationToken cancellationToken)
        {
            // Use lightweight query that only selects IsAiReviewEnabled to avoid SQL issues with includes
            var isEnabled = await _repository.GetAiReviewEnabledByIdAsync(request.TestOrderId, cancellationToken);
            if (isEnabled == null)
                throw new Exception("Test order not found.");

            return isEnabled.Value;
        }
    }
}
