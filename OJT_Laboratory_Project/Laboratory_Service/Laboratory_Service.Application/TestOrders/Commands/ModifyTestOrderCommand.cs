using System.Text.Json.Serialization;
using MediatR;

namespace Laboratory_Service.Application.TestOrders.Commands
{
    /// <summary>
    /// Create attribute for ModifyTestOrderCommand
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;System.Boolean&gt;" />
    public class ModifyTestOrderCommand : IRequest<bool>
    {
        /// <summary>
        /// Gets or sets the test order identifier.
        /// </summary>
        /// <value>
        /// The test order identifier.
        /// </value>
        public Guid TestOrderId { get; set; }
        /// <summary>
        /// Gets or sets the identify number.
        /// </summary>
        /// <value>
        /// The identify number.
        /// </value>
        public string IdentifyNumber { get; set; }
        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        public string Priority { get; set; }
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string Status { get; set; }
        /// <summary>
        /// Gets or sets the note.
        /// </summary>
        /// <value>
        /// The note.
        /// </value>
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets the updated by.
        /// </summary>
        /// <value>
        /// The updated by.
        /// </value>
        [JsonIgnore]
        public string UpdatedBy { get; set; } = string.Empty;
    }
}
