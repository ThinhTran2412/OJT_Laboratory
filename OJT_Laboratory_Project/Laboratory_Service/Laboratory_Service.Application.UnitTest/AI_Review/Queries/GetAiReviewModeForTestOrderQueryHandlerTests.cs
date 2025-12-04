using Xunit;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Laboratory_Service.Application.AiReviewForTestOrder.Querry;
using Laboratory_Service.Application.Interface;
using Laboratory_Service.Domain.Entity;

public class GetAiReviewModeForTestOrderQueryHandlerTests
{
    private readonly Mock<ITestOrderRepository> _repo = new();
    private GetAiReviewModeForTestOrderQueryHandler CreateHandler()
        => new GetAiReviewModeForTestOrderQueryHandler(_repo.Object);

    // -------------------------------------------------------------
    // 1. TestOrder không tồn tại → throw Exception
    // -------------------------------------------------------------
    [Fact]
    public async Task Handle_ThrowsException_WhenTestOrderNotFound()
    {
        _repo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((TestOrder?)null);

        var handler = CreateHandler();
        var query = new GetAiReviewModeForTestOrderQuery(Guid.NewGuid());

        var ex = await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
        Assert.Equal("Test order not found.", ex.Message);
    }

    // -------------------------------------------------------------
    // 2. TestOrder tồn tại → trả về IsAiReviewEnabled = true
    // -------------------------------------------------------------
    [Fact]
    public async Task Handle_ReturnsTrue_WhenAiReviewEnabled()
    {
        var orderId = Guid.NewGuid();
        var order = new TestOrder
        {
            TestOrderId = orderId,
            IsAiReviewEnabled = true
        };

        _repo.Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
             .ReturnsAsync(order);

        var handler = CreateHandler();
        var result = await handler.Handle(new GetAiReviewModeForTestOrderQuery(orderId), CancellationToken.None);

        Assert.True(result);
    }

    // -------------------------------------------------------------
    // 3. TestOrder tồn tại → trả về IsAiReviewEnabled = false
    // -------------------------------------------------------------
    [Fact]
    public async Task Handle_ReturnsFalse_WhenAiReviewDisabled()
    {
        var orderId = Guid.NewGuid();
        var order = new TestOrder
        {
            TestOrderId = orderId,
            IsAiReviewEnabled = false
        };

        _repo.Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
             .ReturnsAsync(order);

        var handler = CreateHandler();
        var result = await handler.Handle(new GetAiReviewModeForTestOrderQuery(orderId), CancellationToken.None);

        Assert.False(result);
    }
}
