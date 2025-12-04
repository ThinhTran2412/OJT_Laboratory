using FluentValidation.TestHelper;
using Laboratory_Service.Application.TestOrders.Commands;


/// <summary>
/// Create unit test for DeleteTestOrderCommandValidation
/// </summary>
public class DeleteTestOrderCommandValidationTests
{
    /// <summary>
    /// The validator
    /// </summary>
    private readonly DeleteTestOrderCommandValidation _validator;

    /// <summary>
    /// The valid command
    /// </summary>
    private readonly DeleteTestOrderCommand _validCommand =
        new DeleteTestOrderCommand(
            TestOrderId: Guid.NewGuid(),
            DeletedBy: "Admin User");

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteTestOrderCommandValidationTests"/> class.
    /// </summary>
    public DeleteTestOrderCommandValidationTests()
    {
        _validator = new DeleteTestOrderCommandValidation();
    }

    // --- 1. Test: Valid Command (Happy Path) ---

    /// <summary>
    /// Shoulds the not have validation errors when command is valid.
    /// </summary>
    [Fact]
    public void Should_Not_Have_Validation_Errors_When_Command_Is_Valid()
    {
        // Act
        var result = _validator.TestValidate(_validCommand);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    // --- 2. Test: TestOrderId Field Validation ---

    /// <summary>
    /// Shoulds the have error when test order identifier is empty.
    /// </summary>
    [Fact]
    public void Should_Have_Error_When_TestOrderId_Is_Empty()
    {
        // Arrange
        var command = _validCommand with { TestOrderId = Guid.Empty };

        // Act & Assert
        _validator.TestValidate(command)
            .ShouldHaveValidationErrorFor(x => x.TestOrderId)
            .WithErrorMessage("TestOrderId is required.");
    }

    // --- 3. Test: DeletedBy Field Validation ---

    /// <summary>
    /// Shoulds the have error when deleted by is zero.
    /// </summary>
    [Fact]
    public void Should_Have_Error_When_DeletedBy_Is_Null()
    {
        // Arrange
        var command = _validCommand with { DeletedBy = null};

        // Act & Assert
        _validator.TestValidate(command)
            .ShouldHaveValidationErrorFor(x => x.DeletedBy)
            .WithErrorMessage("DeletedBy is required.");
    }
}