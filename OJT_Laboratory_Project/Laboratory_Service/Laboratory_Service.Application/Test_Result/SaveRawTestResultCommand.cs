using MediatR;

namespace Laboratory_Service.Application.Test_Result
{
    /// <summary>
    /// Command to save raw test result JSON.
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;System.Boolean&gt;" />
    public class SaveRawTestResultCommand : IRequest<bool>
    {
        /// <summary>
        /// Gets the raw json.
        /// </summary>
        /// <value>
        /// The raw json.
        /// </value>
        public string RawJson { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveRawTestResultCommand"/> class.
        /// </summary>
        /// <param name="rawJson">The raw json.</param>
        public SaveRawTestResultCommand(string rawJson)
        {
            RawJson = rawJson;
        }
    }
}
