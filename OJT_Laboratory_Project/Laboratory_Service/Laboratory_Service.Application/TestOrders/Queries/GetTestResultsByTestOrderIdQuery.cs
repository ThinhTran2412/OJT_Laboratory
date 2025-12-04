using Laboratory_Service.Application.DTOs.TestOrders;
using MediatR;

namespace Laboratory_Service.Application.TestOrders.Queries
{
    /// <summary>
    /// Query to get test results by test order ID
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;System.Collections.Generic.List&lt;Laboratory_Service.Application.DTOs.TestOrders.TestResultDto&gt;&gt;" />
    public class GetTestResultsByTestOrderIdQuery : IRequest<List<TestResultDto>>
    {
        /// <summary>
        /// Gets or sets the test order identifier.
        /// </summary>
        /// <value>
        /// The test order identifier.
        /// </value>
        public Guid TestOrderId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetTestResultsByTestOrderIdQuery"/> class.
        /// </summary>
        /// <param name="testOrderId">The test order identifier.</param>
        public GetTestResultsByTestOrderIdQuery(Guid testOrderId)
        {
            TestOrderId = testOrderId;
        }
    }
}
