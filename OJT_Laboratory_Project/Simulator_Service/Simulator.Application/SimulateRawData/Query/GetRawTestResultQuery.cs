using MediatR;
using Simulator.Application.DTOs;

namespace Simulator.Application.SimulateRawData.Query
{
    /// <summary>
    /// Creates the get raw test result query
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;Simulator.Application.DTOs.RawTestResultDTO&gt;" />
    /// <seealso cref="MediatR.IBaseRequest" />
    /// <seealso cref="System.IEquatable&lt;Simulator.Application.SimulateRawData.Query.GetRawTestResultQuery&gt;" />
    public record GetRawTestResultQuery(Guid TestOrderId) : IRequest<RawTestResultDTO?>;
}
