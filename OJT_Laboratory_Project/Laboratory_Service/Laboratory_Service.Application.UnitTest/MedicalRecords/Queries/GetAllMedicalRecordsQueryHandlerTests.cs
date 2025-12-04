//using Laboratory_Service.Application.Interface;
//using Laboratory_Service.Application.MedicalRecords.Queries;
//using Laboratory_Service.Domain.Entity;
//using Moq;

//public class GetAllMedicalRecordsQueryHandlerTests
//{
//    private readonly Mock<IMedicalRecordRepository> _mockRepository;
//    private readonly GetAllMedicalRecordsQueryHandler _handler;

//    public GetAllMedicalRecordsQueryHandlerTests()
//    {
//        _mockRepository = new Mock<IMedicalRecordRepository>();
//        _handler = new GetAllMedicalRecordsQueryHandler(_mockRepository.Object);
//    }

//    [Fact]
//    public async Task Handle_ShouldReturnMappedMedicalRecordViewDtos()
//    {
//        // Arrange: tạo dữ liệu mẫu
//        var testPatient = new Patient
//        {
//            PatientId = 1,
//            FullName = "John Doe",
//            DateOfBirth = new DateOnly(1990, 1, 1), // Age tự tính
//            Gender = "Male",
//            Address = "123 Street",
//            PhoneNumber = "555-1234",
//            Email = "john@example.com",
//            TestOrders = new List<TestOrder>
//            {
//                new TestOrder
//                {
//                    TestOrderId = Guid.NewGuid(),
//                    OrderCode = "ORD-001",
//                    Priority = "Urgent",
//                    Status = "Pending",
//                    TestType = "Blood Test",
//                    TestResults = null,
//                    RunDate = DateTime.MinValue, // DateTime non-nullable
//                    RunBy = 1,                   // int? hợp lệ
//                    IsDeleted = false
//                }
//            }
//        };

//        var medicalRecords = new List<MedicalRecord>
//        {
//            new MedicalRecord
//            {
//                MedicalRecordId = 1,
//                CreatedAt = DateTime.UtcNow.AddDays(-1),
//                UpdatedAt = DateTime.UtcNow,
//                CreatedBy = "Admin",
//                UpdatedBy = "Admin",
//                Patient = testPatient
//            }
//        };

//        _mockRepository.Setup(r => r.GetAllAsync())
//            .ReturnsAsync(medicalRecords);

//        var query = new GetAllMedicalRecordsQuery();

//        // Act
//        var result = await _handler.Handle(query, CancellationToken.None);

//        // Assert
//        Assert.Single(result);
//        var dto = result.First();
//        Assert.Equal(1, dto.MedicalRecordId);
//        Assert.Equal("John Doe", dto.PatientName);
//        Assert.Equal("Male", dto.Gender);
//        Assert.Single(dto.TestOrders);

//        var testOrderDto = dto.TestOrders.First();
//        Assert.Equal("ORD-001", testOrderDto.OrderCode);
//        Assert.Equal("Pending", testOrderDto.Status);

//        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
//    }

//    [Fact]
//    public async Task Handle_ShouldReturnEmptyList_WhenNoMedicalRecordsExist()
//    {
//        // Arrange
//        _mockRepository.Setup(r => r.GetAllAsync())
//            .ReturnsAsync(new List<MedicalRecord>());

//        var query = new GetAllMedicalRecordsQuery();

//        // Act
//        var result = await _handler.Handle(query, CancellationToken.None);

//        // Assert
//        Assert.Empty(result);
//        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
//    }
//}
