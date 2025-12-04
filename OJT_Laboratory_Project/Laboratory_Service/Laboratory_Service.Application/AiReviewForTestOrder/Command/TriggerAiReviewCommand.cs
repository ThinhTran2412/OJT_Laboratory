using Laboratory_Service.Domain.Entity;
using MediatR;

namespace Laboratory_Service.Application.AiReviewForTestOrder.Command
{
    /// <summary>
    /// Create TriggerAiReviewCommand
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;Laboratory_Service.Domain.Entity.TestOrder&gt;" />
    /// <seealso cref="MediatR.IBaseRequest" />
    /// <seealso cref="System.IEquatable&lt;Laboratory_Service.Application.AiReviewForTestOrder.Command.TriggerAiReviewCommand&gt;" />
    public record TriggerAiReviewCommand(Guid TestOrderId) : IRequest<TestOrder?>;
}
