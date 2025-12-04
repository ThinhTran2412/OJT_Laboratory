using MediatR;
using Simulator.Application.DTOs;

namespace Simulator.Application.SimulateRawData.Query
{
    /// <summary>
    /// Creates the get unsent test results query
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;System.Collections.Generic.IEnumerable&lt;Simulator.Application.DTOs.RawTestResultDTO&gt;&gt;" />
    /// <seealso cref="MediatR.IBaseRequest" />
    /// <seealso cref="System.IEquatable&lt;Simulator.Application.SimulateRawData.Query.GetUnsentTestResultsQuery&gt;" />
    public record GetUnsentTestResultsQuery() : IRequest<IEnumerable<RawTestResultDTO>>;
}
