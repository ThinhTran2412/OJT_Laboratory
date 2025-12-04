using MediatR;
using Simulator.Application.DTOs;

namespace Simulator.Application.SimulateRawData.Command
{
    /// <summary>
    /// Creates the simulate raw data command
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;Simulator.Application.DTOs.RawTestResultDTO&gt;" />
    /// <seealso cref="MediatR.IBaseRequest" />
    /// <seealso cref="System.IEquatable&lt;Simulator.Application.SimulateRawData.Command.SimulateRawDataCommand&gt;" />
    public record SimulateRawDataCommand(Guid TestOrderId) : IRequest<RawTestResultDTO>;
}
