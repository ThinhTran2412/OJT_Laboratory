using Laboratory_Service.Application.Interface;
using Laboratory_Service.Application.TestOrders.Commands;
using Laboratory_Service.Domain.Entity;
using Moq;

/// <summary>
/// Create use case test for DeleteTestOrderCommandHandler
/// </summary>
public class DeleteTestOrderCommandHandlerTests
{
    /// <summary>
    /// The mock repository
    /// </summary>
    private readonly Mock<ITestOrderRepository> _mockRepository;
    /// <summary>
    /// The mock event log service
    /// </summary>
    private readonly Mock<IEventLogService> _mockEventLogService;
    /// <summary>
    /// The handler
    /// </summary>
    private readonly DeleteTestOrderCommandHandler _handler;

    // Command DTO đã được cập nhật để chỉ chứa string cho DeletedBy
    /// <summary>
    /// The valid command
    /// </summary>
    private readonly DeleteTestOrderCommand _validCommand =
        new DeleteTestOrderCommand(
            TestOrderId: Guid.NewGuid(),
            DeletedBy: "Admin User"); // CHỈ SỬ DỤNG STRING

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteTestOrderCommandHandlerTests"/> class.
    /// </summary>
    public DeleteTestOrderCommandHandlerTests()
    {
        // Initialize Mocks and Handler
        _mockRepository = new Mock<ITestOrderRepository>();
        _mockEventLogService = new Mock<IEventLogService>();

        _handler = new DeleteTestOrderCommandHandler(
            _mockRepository.Object,
            _mockEventLogService.Object
        );
    }

    // --- 1. Test: Soft Delete (Status is "Completed") ---

    /// <summary>
    /// Handles the should perform soft delete when status is completed.
    /// </summary>
    [Fact]
    public async Task Handle_Should_PerformSoftDelete_When_StatusIsCompleted()
    {
        // Arrange
        var orderId = _validCommand.TestOrderId;
        var deletedByUsername = _validCommand.DeletedBy; // Lấy giá trị string

        var mockOrder = new TestOrder
        {
            TestOrderId = orderId,
            OrderCode = "ORD-001",
            Status = "Completed",
            IsDeleted = false
            // Lưu ý: Các trường string cho CreatedBy/DeletedBy cần được khởi tạo 
            // trong entity hoặc được gán ở đây nếu cần cho các logic khác
        };

        _mockRepository.Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockOrder);

        // Act
        var result = await _handler.Handle(_validCommand, CancellationToken.None);

        // Assert
        Assert.True(result);

        // 1. Verify Soft Delete (UpdateAsync called)
        _mockRepository.Verify(r => r.UpdateAsync(
            It.Is<TestOrder>(o =>
                o.TestOrderId == orderId &&
                o.IsDeleted == true &&
                o.DeletedBy == deletedByUsername && // VERIFY STRING
                o.DeletedAt.HasValue
            )), Times.Once);

        // 2. Verify Hard Delete (DeleteAsync NOT called)
        _mockRepository.Verify(r => r.DeleteAsync(
            It.IsAny<TestOrder>()), Times.Never);

        // 3. Verify Event Log was created
        _mockEventLogService.Verify(s => s.CreateAsync(
            It.Is<EventLog>(el =>
                el.EventId == "E_00003" &&
                el.EntityId == orderId &&
                el.OperatorName == deletedByUsername // VERIFY STRING
            )), Times.Once);
    }

    // --- 2. Test: Hard Delete (Status is NOT "Completed") ---

    /// <summary>
    /// Handles the should perform hard delete when status is not completed.
    /// </summary>
    [Fact]
    public async Task Handle_Should_PerformHardDelete_When_StatusIsNotCompleted()
    {
        // Arrange
        var orderId = _validCommand.TestOrderId;
        var mockOrder = new TestOrder
        {
            TestOrderId = orderId,
            OrderCode = "ORD-002",
            Status = "Pending",
            IsDeleted = false
        };

        _mockRepository.Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockOrder);

        // Act
        var result = await _handler.Handle(_validCommand, CancellationToken.None);

        // Assert
        Assert.True(result);

        // 1. Verify Hard Delete (DeleteAsync called)
        _mockRepository.Verify(r => r.DeleteAsync(
            It.Is<TestOrder>(o => o.TestOrderId == orderId)), Times.Once);

        // 2. Verify Soft Delete (UpdateAsync NOT called)
        _mockRepository.Verify(r => r.UpdateAsync(
            It.IsAny<TestOrder>()), Times.Never);

        // 3. Verify Event Log was created (Kiểm tra xem EventLog có được tạo với OperatorName là string không)
        _mockEventLogService.Verify(s => s.CreateAsync(
            It.Is<EventLog>(el => el.OperatorName == _validCommand.DeletedBy)), Times.Once);
    }

    // --- 3. Test: Exception Cases ---

    /// <summary>
    /// Handles the should throw key not found exception when test order not found.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ThrowKeyNotFoundException_When_TestOrderNotFound()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TestOrder)null!);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(_validCommand, CancellationToken.None));

        _mockRepository.Verify(r => r.UpdateAsync(
            It.IsAny<TestOrder>()), Times.Never);

        _mockEventLogService.Verify(s => s.CreateAsync(
            It.IsAny<EventLog>()), Times.Never);
    }

    /// <summary>
    /// Handles the should throw invalid operation exception when test order already deleted.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ThrowInvalidOperationException_When_TestOrderAlreadyDeleted()
    {
        // Arrange
        var orderId = _validCommand.TestOrderId;
        var mockOrder = new TestOrder
        {
            TestOrderId = orderId,
            Status = "Pending",
            IsDeleted = true
        };

        _mockRepository.Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockOrder);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(_validCommand, CancellationToken.None));

        _mockRepository.Verify(r => r.DeleteAsync(
            It.IsAny<TestOrder>()), Times.Never);

        _mockEventLogService.Verify(s => s.CreateAsync(
            It.IsAny<EventLog>()), Times.Never);
    }
}