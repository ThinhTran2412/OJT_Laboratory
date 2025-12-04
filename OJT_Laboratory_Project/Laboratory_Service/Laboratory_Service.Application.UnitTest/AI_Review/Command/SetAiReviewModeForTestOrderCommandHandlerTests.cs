using Laboratory_Service.Application.Interface;
using Laboratory_Service.Application.AiReviewForTestOrder.Command;
using Laboratory_Service.Domain.Entity;
using MediatR;
using Moq;

public class SetAiReviewModeForTestOrderCommandHandlerTests
{
    private readonly Mock<ITestOrderRepository> _mockRepository;
    private readonly SetAiReviewModeForTestOrderCommandHandler _handler;

    public SetAiReviewModeForTestOrderCommandHandlerTests()
    {
        _mockRepository = new Mock<ITestOrderRepository>();
        _handler = new SetAiReviewModeForTestOrderCommandHandler(_mockRepository.Object);
    }

    // ------------------------------
    // 1. SUCCESS: Enable = true
    // ------------------------------

    [Fact]
    public async Task Handle_Should_EnableAiReview_When_RequestEnableIsTrue()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var command = new SetAiReviewModeForTestOrderCommand(orderId, true);

        var mockOrder = new TestOrder
        {
            TestOrderId = orderId,
            IsAiReviewEnabled = false
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockOrder);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(Unit.Value, result);
        Assert.True(mockOrder.IsAiReviewEnabled);

        // verify update
        _mockRepository.Verify(r => r.UpdateAsync(
            It.Is<TestOrder>(o =>
                o.TestOrderId == orderId &&
                o.IsAiReviewEnabled == true
            )), Times.Once);
    }

    // ------------------------------
    // 2. SUCCESS: Enable = false
    // ------------------------------

    [Fact]
    public async Task Handle_Should_DisableAiReview_When_RequestEnableIsFalse()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var command = new SetAiReviewModeForTestOrderCommand(orderId, false);

        var mockOrder = new TestOrder
        {
            TestOrderId = orderId,
            IsAiReviewEnabled = true
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockOrder);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(Unit.Value, result);
        Assert.False(mockOrder.IsAiReviewEnabled);

        _mockRepository.Verify(r => r.UpdateAsync(
            It.Is<TestOrder>(o =>
                o.TestOrderId == orderId &&
                o.IsAiReviewEnabled == false
            )), Times.Once);
    }

    // ------------------------------
    // 3. ERROR: Test order not found
    // ------------------------------

    [Fact]
    public async Task Handle_Should_ThrowException_When_TestOrderNotFound()
    {
        // Arrange
        var command = new SetAiReviewModeForTestOrderCommand(Guid.NewGuid(), true);

        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TestOrder)null!);

        // Act + Assert
        await Assert.ThrowsAsync<Exception>(() =>
            _handler.Handle(command, CancellationToken.None));

        // Verify repository never updates
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<TestOrder>()), Times.Never);
    }
}
