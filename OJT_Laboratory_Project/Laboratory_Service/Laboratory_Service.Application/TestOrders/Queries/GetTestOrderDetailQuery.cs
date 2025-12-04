using Laboratory_Service.Application.DTOs.TestOrders;
using MediatR;

namespace Laboratory_Service.Application.TestOrders.Queries
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;Laboratory_Service.Application.DTOs.TestOrders.TestOrderDetailDto&gt;" />
    public class GetTestOrderDetailQuery : IRequest<TestOrderDetailDto>
    {
        /// <summary>
        /// Gets or sets the test order identifier.
        /// </summary>
        /// <value>
        /// The test order identifier.
        /// </value>
        public Guid TestOrderId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetTestOrderDetailQuery"/> class.
        /// </summary>
        /// <param name="testOrderId">The test order identifier.</param>
        public GetTestOrderDetailQuery(Guid testOrderId)
        {
            TestOrderId = testOrderId;
        }
    }
}
