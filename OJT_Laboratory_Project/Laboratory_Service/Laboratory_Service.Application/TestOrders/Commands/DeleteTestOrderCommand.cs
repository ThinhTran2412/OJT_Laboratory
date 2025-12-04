using MediatR;

namespace Laboratory_Service.Application.TestOrders.Commands
{
    /// <summary>
    /// Create record DeleteTestOrderCommand
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;System.Boolean&gt;" />
    /// <seealso cref="MediatR.IBaseRequest" />
    /// <seealso cref="System.IEquatable&lt;Laboratory_Service.Application.TestOrders.Commands.DeleteTestOrderCommand&gt;" />
    public record DeleteTestOrderCommand(Guid TestOrderId, string DeletedBy) : IRequest<bool>;
}
