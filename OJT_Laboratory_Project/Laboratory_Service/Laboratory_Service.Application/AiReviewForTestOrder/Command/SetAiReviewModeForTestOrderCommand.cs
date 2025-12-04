using MediatR;

namespace Laboratory_Service.Application.AiReviewForTestOrder.Command
{
    /// <summary>
    /// Create SetAiReviewModeForTestOrderCommand
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;MediatR.Unit&gt;" />
    /// <seealso cref="MediatR.IBaseRequest" />
    /// <seealso cref="System.IEquatable&lt;Laboratory_Service.Application.AiReviewForTestOrder.Command.SetAiReviewModeForTestOrderCommand&gt;" />
    public record SetAiReviewModeForTestOrderCommand(Guid TestOrderId, bool Enable) : IRequest<Unit>;

}
