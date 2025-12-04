using MediatR;

namespace Laboratory_Service.Application.Test_Result
{
    /// <summary>
    /// create attribute for SyncUpTestOrderCommand
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;System.Boolean&gt;" />
    public class SyncUpTestOrderCommand : IRequest<bool>
    {
        /// <summary>
        /// Gets the raw json.
        /// </summary>
        /// <value>
        /// The raw json.
        /// </value>
        public string RawJson { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncUpTestOrderCommand"/> class.
        /// </summary>
        /// <param name="rawJson">The raw json.</param>
        public SyncUpTestOrderCommand(string rawJson)
        {
            RawJson = rawJson;
        }
    }
}
