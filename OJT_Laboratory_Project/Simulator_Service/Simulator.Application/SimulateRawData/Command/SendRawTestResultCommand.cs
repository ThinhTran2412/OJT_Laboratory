using MediatR;
using Simulator.Application.DTOs;

namespace Simulator.Application.SimulateRawData.Command
{
    /// <summary>
    /// Creates the send raw test result command
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;Simulator.Application.SimulateRawData.Command.SendRawTestResultResult&gt;" />
    /// <seealso cref="MediatR.IBaseRequest" />
    /// <seealso cref="System.IEquatable&lt;Simulator.Application.SimulateRawData.Command.SendRawTestResultCommand&gt;" />
    public record SendRawTestResultCommand(RawTestResultDTO RawResult) : IRequest<SendRawTestResultResult>;
}
