using Laboratory_Service.Domain.Entity;
using MediatR;

namespace Laboratory_Service.Application.AiReviewForTestOrder.Command
{
    /// <summary>
    /// Create ConfirmAiReviewResults
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;Laboratory_Service.Domain.Entity.TestOrder&gt;" />
    /// <seealso cref="MediatR.IBaseRequest" />
    /// <seealso cref="System.IEquatable&lt;Laboratory_Service.Application.AiReviewForTestOrder.Command.ConfirmAiReviewResultsCommand&gt;" />
    public record ConfirmAiReviewResultsCommand(Guid TestOrderId, int ConfirmedByUserId) : IRequest<TestOrder?>;
}
