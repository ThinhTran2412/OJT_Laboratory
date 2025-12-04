using Laboratory_Service.Application.DTOs.TestOrders;
using MediatR;

namespace Laboratory_Service.Application.TestOrders.Queries
{
    /// <summary>
    /// Query to get all test orders for a specific patient by patient ID.
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;System.Collections.Generic.IEnumerable&lt;Laboratory_Service.Application.DTOs.TestOrders.TestOrderListItemDto&gt;&gt;" />
    public class GetTestOrdersByPatientIdQuery : IRequest<IEnumerable<TestOrderListItemDto>>
    {
        /// <summary>
        /// Gets or sets the patient identifier.
        /// </summary>
        /// <value>
        /// The patient identifier.
        /// </value>
        public int PatientId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetTestOrdersByPatientIdQuery"/> class.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        public GetTestOrdersByPatientIdQuery(int patientId)
        {
            PatientId = patientId;
        }
    }
}

