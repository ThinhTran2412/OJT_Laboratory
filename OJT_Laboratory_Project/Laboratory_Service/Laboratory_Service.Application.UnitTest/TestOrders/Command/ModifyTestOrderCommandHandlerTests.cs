//using Laboratory_Service.Application.Interface;
//using Laboratory_Service.Application.TestOrders.Commands;
//using Laboratory_Service.Domain.Entity;
//using Microsoft.Extensions.Logging;
//using Moq;

///// <summary>
///// Unit tests for ModifyTestOrderCommandHandler
///// </summary>
//public class ModifyTestOrderCommandHandlerTests
//{
//    private readonly Mock<ITestOrderRepository> _mockOrderRepo;
//    private readonly Mock<IMedicalRecordRepository> _mockRecordRepo;
//    private readonly Mock<IPatientService> _mockPatientService;
//    private readonly Mock<IEventLogService> _mockEventLogService;
//    private readonly Mock<ILogger<ModifyTestOrderCommandHandler>> _mockLogger;
//    private readonly ModifyTestOrderCommandHandler _handler;

//    private readonly Guid _testOrderId = Guid.NewGuid();
//    private readonly int _patientId = 42;

//    private readonly Patient _syncedPatient;
//    private readonly ModifyTestOrderCommand _validCommand;
//    private readonly TestOrder _initialOrder;

//    public ModifyTestOrderCommandHandlerTests()
//    {
//        // Initialize Mocks
//        _mockOrderRepo = new Mock<ITestOrderRepository>();
//        _mockRecordRepo = new Mock<IMedicalRecordRepository>();
//        _mockPatientService = new Mock<IPatientService>();
//        _mockEventLogService = new Mock<IEventLogService>();
//        _mockLogger = new Mock<ILogger<ModifyTestOrderCommandHandler>>();

//        // Setup Base Entities
//        _syncedPatient = new Patient { PatientId = _patientId, IdentifyNumber = "123456789" };

//        _initialOrder = new TestOrder
//        {
//            TestOrderId = _testOrderId,
//            OrderCode = "OLD-001",
//            PatientId = _patientId,
//            Status = "Pending",
//            Priority = "Normal",
//            Note = "Original note."
//        };

//        // Setup Valid Command
//        _validCommand = new ModifyTestOrderCommand
//        {
//            TestOrderId = _testOrderId,
//            IdentifyNumber = "123456789",
//            PatientName = "New Name",
//            DateOfBirth = new DateOnly(1990, 1, 1),
//            Age = 33,
//            Gender = "Male",
//            Address = "New Address",
//            PhoneNumber = "555-1234",
//            Email = "new@email.com",
//            Priority = "Urgent",
//            Status = "Processing",
//            Note = "Updated note.",
//            UpdatedBy = 99,
//            UpdatedByName = "Modifier"
//        };

//        // Setup Dependencies
//        _mockPatientService
//            .Setup(s => s.SynchronizePatientWithUserAsync(
//                It.IsAny<string>(),
//                It.IsAny<string>()))
//            .ReturnsAsync(_syncedPatient);

//        // Initialize Handler
//        _handler = new ModifyTestOrderCommandHandler(
//            _mockOrderRepo.Object,
//            _mockRecordRepo.Object,
//            _mockPatientService.Object,
//            _mockEventLogService.Object,
//            _mockLogger.Object
//        );
//    }

//    // Scenario 1: Order and record exist
//    [Fact]
//    public async Task Handle_Should_UpdateOrder_AndNotAddRecord_When_RecordExists()
//    {
//        // Arrange
//        _mockOrderRepo
//            .Setup(r => r.GetByIdAsync(_testOrderId, It.IsAny<CancellationToken>()))
//            .ReturnsAsync(_initialOrder);

//        _mockRecordRepo
//            .Setup(r => r.GetByPatientIdAsync(_patientId))
//            .ReturnsAsync(new List<MedicalRecord> { new MedicalRecord() });

//        // Act
//        var result = await _handler.Handle(_validCommand, CancellationToken.None);

//        // Assert
//        Assert.True(result);

//        _mockPatientService.Verify(s => s.SynchronizePatientWithUserAsync(
//            _validCommand.IdentifyNumber,
//            _validCommand.UpdatedBy.ToString()), Times.Once);

//        _mockRecordRepo.Verify(r => r.AddAsync(It.IsAny<MedicalRecord>()), Times.Never);
//        _mockOrderRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

//        _mockEventLogService.Verify(s => s.CreateAsync(
//            It.Is<EventLog>(el =>
//                el.Action == "Modify Test Order" &&
//                el.OperatorId == _validCommand.UpdatedBy
//            )), Times.Once);

//        Assert.Equal(_validCommand.Status, _initialOrder.Status);
//        Assert.Equal(_validCommand.Priority, _initialOrder.Priority);
//        Assert.Equal(_validCommand.UpdatedBy, _initialOrder.UpdatedBy);
//    }

//    // Scenario 2: No medical record exists
//    [Fact]
//    public async Task Handle_Should_AddMedicalRecord_When_NoRecordFound()
//    {
//        // Arrange
//        _mockOrderRepo
//            .Setup(r => r.GetByIdAsync(_testOrderId, It.IsAny<CancellationToken>()))
//            .ReturnsAsync(_initialOrder);

//        _mockRecordRepo
//            .Setup(r => r.GetByPatientIdAsync(_patientId))
//            .ReturnsAsync(new List<MedicalRecord>());

//        // Act
//        await _handler.Handle(_validCommand, CancellationToken.None);

//        // Assert
//        _mockRecordRepo.Verify(r => r.AddAsync(
//            It.Is<MedicalRecord>(mr =>
//                mr.PatientId == _patientId &&
//                mr.CreatedBy == _validCommand.UpdatedBy.ToString()
//            )), Times.Once);

//        _mockOrderRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//    }

//    // Scenario 3: Patient ID changes during sync
//    [Fact]
//    public async Task Handle_Should_UpdateOrderPatientId_When_SyncedPatientIsDifferent()
//    {
//        // Arrange
//        var oldPatientId = 999;
//        var newPatientId = _syncedPatient.PatientId;

//        var order = new TestOrder
//        {
//            TestOrderId = _testOrderId,
//            PatientId = oldPatientId,
//            Status = "Pending"
//        };

//        _mockOrderRepo
//            .Setup(r => r.GetByIdAsync(_testOrderId, It.IsAny<CancellationToken>()))
//            .ReturnsAsync(order);

//        _mockRecordRepo
//            .Setup(r => r.GetByPatientIdAsync(newPatientId))
//            .ReturnsAsync(new List<MedicalRecord> { new MedicalRecord() });

//        // Act
//        await _handler.Handle(_validCommand, CancellationToken.None);

//        // Assert
//        Assert.Equal(newPatientId, order.PatientId);

//        _mockLogger.Verify(
//            x => x.Log(
//                LogLevel.Information,
//                It.IsAny<EventId>(),
//                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Re-linked TestOrder to PatientId")),
//                It.IsAny<Exception>(),
//                It.IsAny<Func<It.IsAnyType, Exception?, string>>()!),
//            Times.Once);
//    }

//    // Scenario 4: Order not found
//    [Fact]
//    public async Task Handle_Should_ThrowException_When_OrderNotFound()
//    {
//        // Arrange
//        _mockOrderRepo
//            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync((TestOrder)null!);

//        // Act and Assert
//        await Assert.ThrowsAsync<InvalidOperationException>(() =>
//            _handler.Handle(_validCommand, CancellationToken.None));

//        _mockPatientService.Verify(s => s.SynchronizePatientWithUserAsync(
//            It.IsAny<string>(), It.IsAny<string>()), Times.Never);
//    }

//    // Scenario 5: Patient synchronization fails
//    [Fact]
//    public async Task Handle_Should_ThrowException_When_PatientSyncFails()
//    {
//        // Arrange
//        _mockOrderRepo
//            .Setup(r => r.GetByIdAsync(_testOrderId, It.IsAny<CancellationToken>()))
//            .ReturnsAsync(_initialOrder);

//        _mockPatientService
//            .Setup(s => s.SynchronizePatientWithUserAsync(
//                It.IsAny<string>(),
//                It.IsAny<string>()))
//            .ReturnsAsync((Patient)null!);

//        // Act and Assert
//        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
//            _handler.Handle(_validCommand, CancellationToken.None));

//        Assert.Contains("Cannot synchronize patient with IAM Service", exception.Message);

//        _mockOrderRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
//    }
//}
