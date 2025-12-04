using Laboratory_Service.Application.AiReviewForTestOrder.Command;
using Laboratory_Service.Application.Interface;
using Laboratory_Service.Domain.Entity;
using Moq;

public class ConfirmAiReviewResultsCommandHandlerTests
{
    private readonly Mock<ITestOrderRepository> _mockTestOrderRepo;
    private readonly Mock<ITestResultRepository> _mockTestResultRepo;
    private readonly ConfirmAiReviewResultsCommandHandler _handler;

    private readonly ConfirmAiReviewResultsCommand _validCommand =
        new ConfirmAiReviewResultsCommand(
            TestOrderId: Guid.NewGuid(),
            ConfirmedByUserId: 123
        );

    public ConfirmAiReviewResultsCommandHandlerTests()
    {
        _mockTestOrderRepo = new Mock<ITestOrderRepository>();
        _mockTestResultRepo = new Mock<ITestResultRepository>();

        _handler = new ConfirmAiReviewResultsCommandHandler(
            _mockTestOrderRepo.Object,
            _mockTestResultRepo.Object
        );
    }

    // ===============================================================
    // 1. Test: Order not found -> return null
    // ===============================================================

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenOrderNotFound()
    {
        // Arrange
        _mockTestOrderRepo.Setup(r => r.GetByIdWithResultsAsync(
                It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TestOrder)null!);

        // Act
        var result = await _handler.Handle(_validCommand, CancellationToken.None);

        // Assert
        Assert.Null(result);

        _mockTestResultRepo.Verify(r =>
            r.UpdateRangeAsync(It.IsAny<IEnumerable<TestResult>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ===============================================================
    // 2. Test: Status != "Reviewed By AI" -> throw
    // ===============================================================

    [Fact]
    public async Task Handle_ShouldThrow_WhenStatusIsNotReviewedByAI()
    {
        // Arrange
        var order = new TestOrder
        {
            TestOrderId = _validCommand.TestOrderId,
            Status = "Pending",
            TestResults = new List<TestResult>()
        };

        _mockTestOrderRepo.Setup(r => r.GetByIdWithResultsAsync(order.TestOrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act + Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(_validCommand, CancellationToken.None));

        _mockTestResultRepo.Verify(r => r.UpdateRangeAsync(
            It.IsAny<IEnumerable<TestResult>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ===============================================================
    // 3. Test: No results -> throw
    // ===============================================================

    [Fact]
    public async Task Handle_ShouldThrow_WhenOrderHasNoResults()
    {
        // Arrange
        var order = new TestOrder
        {
            TestOrderId = _validCommand.TestOrderId,
            Status = "Reviewed By AI",
            TestResults = new List<TestResult>() // empty
        };

        _mockTestOrderRepo.Setup(r => r.GetByIdWithResultsAsync(order.TestOrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act + Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(_validCommand, CancellationToken.None));

        _mockTestResultRepo.Verify(r => r.UpdateRangeAsync(
            It.IsAny<IEnumerable<TestResult>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ===============================================================
    // 4. Test: No AI-reviewed results to confirm -> throw
    // ===============================================================

    [Fact]
    public async Task Handle_ShouldThrow_WhenNoAiReviewedResultsToConfirm()
    {
        // Arrange
        var order = new TestOrder
        {
            TestOrderId = _validCommand.TestOrderId,
            Status = "Reviewed By AI",
            TestResults = new List<TestResult>
            {
                new TestResult { ReviewedByAI = false, IsConfirmed = false },
                new TestResult { ReviewedByAI = true, IsConfirmed = true } // already confirmed
            }
        };

        _mockTestOrderRepo.Setup(r => r.GetByIdWithResultsAsync(order.TestOrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act + Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(_validCommand, CancellationToken.None));

        _mockTestResultRepo.Verify(r => r.UpdateRangeAsync(
            It.IsAny<IEnumerable<TestResult>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ===============================================================
    // 5. Test: Successfully confirm AI-reviewed results
    // ===============================================================

    [Fact]
    public async Task Handle_ShouldConfirmResults_WhenValid()
    {
        // Arrange
        var r1 = new TestResult
        {
            ReviewedByAI = true,
            IsConfirmed = false
        };
        var r2 = new TestResult
        {
            ReviewedByAI = true,
            IsConfirmed = false
        };
        var r3 = new TestResult
        {
            ReviewedByAI = false,
            IsConfirmed = false
        };

        var order = new TestOrder
        {
            TestOrderId = _validCommand.TestOrderId,
            Status = "Reviewed By AI",
            TestResults = new List<TestResult> { r1, r2, r3 }
        };

        _mockTestOrderRepo.Setup(r => r.GetByIdWithResultsAsync(order.TestOrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _handler.Handle(_validCommand, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(r1.IsConfirmed);
        Assert.True(r2.IsConfirmed);
        Assert.False(r3.IsConfirmed); // không phải AI-reviewed

        Assert.Equal(123, r1.ConfirmedByUserId);
        Assert.Equal(123, r2.ConfirmedByUserId);


        _mockTestResultRepo.Verify(r =>
            r.UpdateRangeAsync(
                It.Is<IEnumerable<TestResult>>(list =>
                    list.Contains(r1) &&
                    list.Contains(r2) &&
                    !list.Contains(r3) // result 3 không được confirm
                ),
                It.IsAny<CancellationToken>()
            ),
            Times.Once);
    }
}
