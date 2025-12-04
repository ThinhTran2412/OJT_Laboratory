using Laboratory_Service.Application.Interface;
using Laboratory_Service.Application.TestOrders.Queries;
using Laboratory_Service.Domain.Entity;
using Moq;

namespace Laboratory_Service.Application.UnitTest.TestOrders.Query;

public class GetTestOrderDetailQueryHandlerTests
{
    private readonly Mock<ITestOrderRepository> _orderRepoMock = new();
    private readonly GetTestOrderDetailQueryHandler _handler;

    public GetTestOrderDetailQueryHandlerTests()
    {
        _handler = new GetTestOrderDetailQueryHandler(_orderRepoMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsDto_When_OrderExists()
    {
        var id = Guid.NewGuid();
        var order = BuildOrder(id,
            patient: new Patient
            {
                PatientId = 10,
                FullName = "John Doe",
                Gender = "Male",
                PhoneNumber = "0900000000",
                IdentifyNumber = "CCCD-123456789",
                DateOfBirth = new DateOnly(1990, 1, 1)
            },
            createdAt: DateTime.UtcNow.AddDays(-1),
            runDate: DateTime.UtcNow);

        _orderRepoMock.Setup(r => r.GetByIdWithResultsAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var result = await _handler.Handle(new GetTestOrderDetailQuery(id), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(id, result!.TestOrderId);
        Assert.Equal(order.MedicalRecord!.PatientId, result.PatientId);
        Assert.Equal(order.MedicalRecord.Patient.FullName, result.PatientName);
        Assert.Equal(order.MedicalRecord.Patient.Gender, result.Gender);
        Assert.Equal(order.MedicalRecord.Patient.PhoneNumber, result.PhoneNumber);
        Assert.Equal(order.MedicalRecord.Patient.IdentifyNumber, result.IdentifyNumber);
        Assert.Equal(order.Status, result.Status);
        Assert.Equal(order.CreatedAt, result.CreatedAt);
        Assert.Equal(order.RunDate, result.RunDate);
        Assert.Equal(order.TestType, result.TestType);
    }

    [Fact]
    public async Task Handle_ReturnsDto_WithTestResults_When_OrderContainsResults()
    {
        var id = Guid.NewGuid();
        var order = BuildOrder(id, runBy: 777, testResults: new()
        {
            new TestResult
            {
                TestResultId = 2,
                TestOrderId = id,
                TestCode = "WBC",
                Flag = "High",
                ResultStatus = "Completed",
                ReviewedByUserId = 102,
                ReviewedDate = DateTime.UtcNow
            },
            new TestResult
            {
                TestResultId = 1,
                TestOrderId = id,
                TestCode = "HB",
                Flag = "Normal",
                ResultStatus = "Completed",
                ReviewedByUserId = 101,
                ReviewedDate = DateTime.UtcNow.AddMinutes(-5)
            }
        });

        _orderRepoMock.Setup(r => r.GetByIdWithResultsAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var result = await _handler.Handle(new GetTestOrderDetailQuery(id), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result!.TestResults.Count);
        Assert.Equal("WBC", result.TestResults[0].TestCode);
        Assert.Equal(order.RunBy, result.TestResults[0].PerformedBy);
        Assert.Equal(order.RunDate, result.TestResults[0].PerformedDate);
        Assert.Equal("High", result.TestResults[0].Status);
        Assert.Equal("HB", result.TestResults[1].TestCode);
        Assert.Equal(101, result.TestResults[1].ReviewedBy); // ReviewedBy in DTO maps from ReviewedByUserId
    }

    [Fact]
    public async Task Handle_ReturnsNull_When_OrderNotFound()
    {
        var id = Guid.NewGuid();
        _orderRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TestOrder?)null);

        var result = await _handler.Handle(new GetTestOrderDetailQuery(id), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_ReturnsNull_When_OrderIsDeleted()
    {
        var id = Guid.NewGuid();
        var order = BuildOrder(id);
        order.IsDeleted = true;

        _orderRepoMock.Setup(r => r.GetByIdWithResultsAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var result = await _handler.Handle(new GetTestOrderDetailQuery(id), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_ReturnsDto_WithNullPatient_When_MedicalRecordIsNull()
    {
        var id = Guid.NewGuid();
        var order = new TestOrder
        {
            TestOrderId = id,
            MedicalRecordId = 1,
            MedicalRecord = null,
            Status = "Created",
            TestType = "Blood Test",
            CreatedAt = DateTime.UtcNow,
            RunDate = DateTime.UtcNow
        };

        _orderRepoMock.Setup(r => r.GetByIdWithResultsAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var result = await _handler.Handle(new GetTestOrderDetailQuery(id), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(0, result!.PatientId);
        Assert.Equal(string.Empty, result.PatientName);
        Assert.Equal(string.Empty, result.Gender);
    }

    [Fact]
    public async Task Handle_ReturnsMessage_When_OrderHasNoResults()
    {
        var id = Guid.NewGuid();
        var order = BuildOrder(id, testResults: new List<TestResult>());

        _orderRepoMock.Setup(r => r.GetByIdWithResultsAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var result = await _handler.Handle(new GetTestOrderDetailQuery(id), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("No test results available for this order.", result!.Message);
        Assert.Empty(result.TestResults);
    }

    private static TestOrder BuildOrder(Guid id,
        Patient? patient = null,
        DateTime? createdAt = null,
        DateTime? runDate = null,
        int? runBy = null,
        List<TestResult>? testResults = null)
    {
        patient ??= new Patient { PatientId = 1, FullName = "Default Patient", Gender = "Female" };
        return new TestOrder
        {
            TestOrderId = id,
            MedicalRecordId = 1,
            MedicalRecord = new MedicalRecord { MedicalRecordId = 1, PatientId = patient.PatientId, Patient = patient },
            OrderCode = "ORD-001",
            Status = "Completed",
            Priority = "Normal",
            TestType = "Blood Test",
            CreatedAt = createdAt ?? DateTime.UtcNow.AddDays(-2),
            RunDate = runDate ?? DateTime.UtcNow,
            RunBy = runBy,
            CreatedBy = "doctor",
            IsDeleted = false,
            TestResults = testResults ?? new List<TestResult>
            {
                new TestResult
                {
                    TestResultId = 1,
                    TestOrderId = id,
                    TestCode = "GLU",
                    Flag = "Normal",
                    ResultStatus = "Completed",
                    ReviewedByUserId = 10,
                    ReviewedDate = DateTime.UtcNow.AddHours(-1)
                }
            }
        };
    }
}


