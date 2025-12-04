using MediatR;

namespace Laboratory_Service.Application.AiReviewForTestOrder.Querry
{
    /// <summary>
    /// Create GetAiReviewModeForTestOrderQuery
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;System.Boolean&gt;" />
    /// <seealso cref="MediatR.IBaseRequest" />
    /// <seealso cref="System.IEquatable&lt;Laboratory_Service.Application.AiReviewForTestOrder.Querry.GetAiReviewModeForTestOrderQuery&gt;" />
    public record GetAiReviewModeForTestOrderQuery(Guid TestOrderId) : IRequest<bool>;
}
