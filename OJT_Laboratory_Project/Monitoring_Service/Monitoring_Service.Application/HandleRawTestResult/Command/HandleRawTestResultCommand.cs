using MediatR;

namespace Monitoring_Service.Application.HandleRawTestResult.Command
{
    /// <summary>
    /// Creates the handle raw test result command.
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;System.Boolean&gt;" />
    /// <seealso cref="MediatR.IBaseRequest" />
    /// <seealso cref="System.IEquatable&lt;Monitoring_Service.Application.HandleRawTestResult.Command.HandleRawTestResultCommand&gt;" />
    public record HandleRawTestResultCommand(string RawJson) : IRequest<bool>;
}
