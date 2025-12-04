//using Laboratory_Service.Application.Interface;
//using Laboratory_Service.Application.TestOrders.Commands;
//using Laboratory_Service.Domain.Entity;
//using Microsoft.Extensions.Logging;
//using Moq;

///// <summary>
///// Create use case test for CreateTestOrderCommandHandler
///// </summary>
//public class CreateTestOrderCommandHandlerTests
//{
//    // Mocking all dependencies used by the provided handler
//    /// <summary>
//    /// The mock iam client
//    /// </summary>
//    private readonly Mock<IIamUserService> _mockIamClient;
//    /// <summary>
//    /// The mock patient repo
//    /// </summary>
//    private readonly Mock<IPatientRepository> _mockPatientRepo;
//    /// <summary>
//    /// The mock record repo
//    /// </summary>
//    private readonly Mock<IMedicalRecordRepository> _mockRecordRepo;
//    /// <summary>
//    /// The mock order repo
//    /// </summary>
//    private readonly Mock<ITestOrderRepository> _mockOrderRepo;
//    /// <summary>
//    /// The mock patient service
//    /// </summary>
//    private readonly Mock<IPatientService> _mockPatientService;
//    /// <summary>
//    /// The mock logger
//    /// </summary>
//    private readonly Mock<ILogger<CreateTestOrderCommandHandler>> _mockLogger;

//    /// <summary>
//    /// The handler
//    /// </summary>
//    private readonly CreateTestOrderCommandHandler _handler;

//    // A reusable, valid command for testing
//    /// <summary>
//    /// The valid command
//    /// </summary>
//    private readonly CreateTestOrderCommand _validCommand = new CreateTestOrderCommand
//    {
//        IdentifyNumber = "ID123456",
//        ServicePackageId = Guid.NewGuid(),
//        Priority = "Normal",
//        Note = "Standard test.",
//        CreatedBy = 101
//    };

//    // A reusable, valid Patient entity
//    /// <summary>
//    /// The mock patient
//    /// </summary>
//    private readonly Patient _mockPatient = new Patient
//    {
//        PatientId = 5,
//        FullName = "John Doe",
//        DateOfBirth = new DateOnly(1980, 5, 15),
//        Gender = "Male",
//        Address = "123 Test St"
//    };


//    /// <summary>
//    /// Initializes a new instance of the <see cref="CreateTestOrderCommandHandlerTests"/> class.
//    /// </summary>
//    public CreateTestOrderCommandHandlerTests()
//    {
//        // Initialize Mocks
//        _mockIamClient = new Mock<IIamUserService>();
//        _mockPatientRepo = new Mock<IPatientRepository>();
//        _mockRecordRepo = new Mock<IMedicalRecordRepository>();
//        _mockOrderRepo = new Mock<ITestOrderRepository>();
//        _mockPatientService = new Mock<IPatientService>();
//        _mockLogger = new Mock<ILogger<CreateTestOrderCommandHandler>>();

//        // Initialize Handler with all Mocks
//        _handler = new CreateTestOrderCommandHandler(
//            _mockIamClient.Object,
//            _mockPatientRepo.Object,
//            _mockRecordRepo.Object,
//            _mockOrderRepo.Object,
//            _mockLogger.Object,
//            _mockPatientService.Object
//        );

//        // Setup base assumptions for the happy path
//        _mockIamClient.Setup(c => c.CheckUserExistsAsync(It.IsAny<string>()))
//            .ReturnsAsync(true);
//    }

//    /// <summary>
//    /// Test case for the 'Happy Path' where the patient already exists, and a medical record exists.
//    /// </summary>
//    [Fact]
//    public async Task Handle_Should_CreateTestOrder_When_PatientAndRecord_Exist()
//    {
//        // Arrange
//        // 1. Patient exists
//        _mockPatientRepo.Setup(r => r.GetByIdentifyNumberAsync(It.IsAny<string>()))
//            .ReturnsAsync(_mockPatient);

//        // 2. Medical Record exists (non-null/non-empty list)
//        _mockRecordRepo.Setup(r => r.GetByPatientIdAsync(_mockPatient.PatientId))
//            .ReturnsAsync(new System.Collections.Generic.List<MedicalRecord> { new MedicalRecord() });

//        // Act
//        var resultGuid = await _handler.Handle(_validCommand, CancellationToken.None);

//        // Assert
//        // 1. Check result
//        Assert.NotEqual(Guid.Empty, resultGuid);

//        // 2. Verify patient service was NOT called to create a new patient
//        _mockPatientService.Verify(s => s.CreatePatientFromUserAsync(
//            It.IsAny<string>(), It.IsAny<string>()), Times.Never);

//        // 3. Verify medical record addition was NOT called
//        _mockRecordRepo.Verify(r => r.AddAsync(
//            It.IsAny<MedicalRecord>()),
//            Times.Never);

//        // 4. Verify TestOrder was added once with correct PatientId
//        _mockOrderRepo.Verify(r => r.AddAsync(
//            It.Is<TestOrder>(order =>
//                order.PatientId == _mockPatient.PatientId &&
//                order.Status == "Created"),
//            It.IsAny<CancellationToken>()), Times.Once);
//    }

//    /// <summary>
//    /// Test case where the patient does NOT exist locally, requiring creation from IAM,
//    /// AND a new medical record is created because none existed.
//    /// </summary>
//    [Fact]
//    public async Task Handle_Should_CreatePatientAndRecord_Then_CreateTestOrder()
//    {
//        // Arrange
//        // 1. Patient does NOT exist locally
//        _mockPatientRepo.Setup(r => r.GetByIdentifyNumberAsync(It.IsAny<string>()))
//            .ReturnsAsync((Patient)null!);

//        // 2. PatientService successfully creates and returns the patient
//        _mockPatientService.Setup(s => s.CreatePatientFromUserAsync(
//            It.IsAny<string>(), It.IsAny<string>()))
//            .ReturnsAsync(_mockPatient);

//        // 3. Medical Record does NOT exist
//        _mockRecordRepo.Setup(r => r.GetByPatientIdAsync(It.IsAny<int>()))
//            .ReturnsAsync((System.Collections.Generic.List<MedicalRecord>)null!);

//        // Act
//        var resultGuid = await _handler.Handle(_validCommand, CancellationToken.None);

//        // Assert
//        // 1. Check result
//        Assert.NotEqual(Guid.Empty, resultGuid);

//        // 2. Verify patient service WAS called to create a new patient
//        _mockPatientService.Verify(s => s.CreatePatientFromUserAsync(
//            _validCommand.IdentifyNumber, _validCommand.CreatedBy.ToString()), Times.Once);

//        // 3. Verify medical record addition WAS called (to create the first record)
//        _mockRecordRepo.Verify(r => r.AddAsync(
//            It.Is<MedicalRecord>(mr => mr.PatientId == _mockPatient.PatientId)
//            ), Times.Once);

//        // 4. Verify TestOrder was added once
//        _mockOrderRepo.Verify(r => r.AddAsync(
//            It.IsAny<TestOrder>(), It.IsAny<CancellationToken>()), Times.Once);
//    }

//    /// <summary>
//    /// Test case for failure when the user does not exist in the IAM Service.
//    /// </summary>
//    [Fact]
//    public async Task Handle_Should_ThrowException_When_UserNotExistsInIAM()
//    {
//        // Arrange
//        // IAM Client returns false (User not found)
//        _mockIamClient.Setup(c => c.CheckUserExistsAsync(It.IsAny<string>()))
//            .ReturnsAsync(false);

//        // Act & Assert
//        await Assert.ThrowsAsync<InvalidOperationException>(
//            () => _handler.Handle(_validCommand, CancellationToken.None));

//        // Ensure no repository/service calls occurred after the check
//        _mockPatientRepo.Verify(r => r.GetByIdentifyNumberAsync(It.IsAny<string>()), Times.Never);
//        _mockOrderRepo.Verify(r => r.AddAsync(It.IsAny<TestOrder>(), It.IsAny<CancellationToken>()), Times.Never);
//    }

//    /// <summary>
//    /// Test case for failure when patient creation is required but the PatientService fails (returns null).
//    /// </summary>
//    [Fact]
//    public async Task Handle_Should_ThrowException_When_CreatePatientFromUserAsync_Fails()
//    {
//        // Arrange
//        // 1. Patient does NOT exist locally
//        _mockPatientRepo.Setup(r => r.GetByIdentifyNumberAsync(It.IsAny<string>()))
//            .ReturnsAsync((Patient)null!);

//        // 2. PatientService fails to create the patient (returns null)
//        _mockPatientService.Setup(s => s.CreatePatientFromUserAsync(
//            It.IsAny<string>(), It.IsAny<string>()))
//            .ReturnsAsync((Patient)null!);

//        // Act & Assert
//        await Assert.ThrowsAsync<InvalidOperationException>(
//            () => _handler.Handle(_validCommand, CancellationToken.None));

//        // Ensure TestOrderRepository was not called
//        _mockOrderRepo.Verify(r => r.AddAsync(It.IsAny<TestOrder>(), It.IsAny<CancellationToken>()), Times.Never);
//    }
//}