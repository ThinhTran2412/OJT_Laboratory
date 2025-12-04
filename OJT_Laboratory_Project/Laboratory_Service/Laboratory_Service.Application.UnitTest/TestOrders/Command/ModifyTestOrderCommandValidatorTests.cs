using FluentValidation.TestHelper;
using Laboratory_Service.Application.TestOrders.Commands;

/// <summary>
/// Create use case for ModifyTestOrderCommandValidator
/// </summary>
public class ModifyTestOrderCommandValidatorTests
{
    /// <summary>
    /// The validator
    /// </summary>
    private readonly ModifyTestOrderCommandValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModifyTestOrderCommandValidatorTests"/> class.
    /// </summary>
    public ModifyTestOrderCommandValidatorTests()
    {
        _validator = new ModifyTestOrderCommandValidator();
    }

    // --- 1. TestOrderId Validation ---

    /// <summary>
    /// Shoulds the have error when test order identifier is empty.
    /// </summary>
    [Fact]
    public void Should_Have_Error_When_TestOrderId_IsEmpty()
    {
        // Arrange
        var command = new ModifyTestOrderCommand { TestOrderId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TestOrderId)
              .WithErrorMessage("TestOrderId is required.");
    }

    /// <summary>
    /// Shoulds the not have error when test order identifier is not empty.
    /// </summary>
    [Fact]
    public void Should_Not_Have_Error_When_TestOrderId_IsNotEmpty()
    {
        // Arrange
        var command = new ModifyTestOrderCommand { TestOrderId = Guid.NewGuid() };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.TestOrderId);
    }

    // --- 2. IdentifyNumber Validation ---

    /// <summary>
    /// Shoulds the have error when identify number is null or empty.
    /// </summary>
    /// <param name="identifyNumber">The identify number.</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_Have_Error_When_IdentifyNumber_IsNullOrEmpty(string identifyNumber)
    {
        // Arrange
        var command = new ModifyTestOrderCommand { IdentifyNumber = identifyNumber };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.IdentifyNumber)
              .WithErrorMessage("IdentifyNumber is required.");
    }

    /// <summary>
    /// Shoulds the have error when identify number has incorrect format.
    /// </summary>
    /// <param name="identifyNumber">The identify number.</param>
    [Theory]
    [InlineData("12345678")] 
    [InlineData("1234567890")]
    [InlineData("12345678901")]
    [InlineData("ABCDEFGHI")] 
    [InlineData("1234567890123")] 
    public void Should_Have_Error_When_IdentifyNumber_Has_Incorrect_Format(string identifyNumber)
    {
        // Arrange
        var command = new ModifyTestOrderCommand { IdentifyNumber = identifyNumber };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.IdentifyNumber)
              .WithErrorMessage("IdentifyNumber must contain 9 or 12 digits.");
    }

    /// <summary>
    /// Shoulds the not have error when identify number is valid.
    /// </summary>
    /// <param name="identifyNumber">The identify number.</param>
    [Theory]
    [InlineData("123456789")] 
    [InlineData("123456789012")] 
    public void Should_Not_Have_Error_When_IdentifyNumber_Is_Valid(string identifyNumber)
    {
        // Arrange
        var command = new ModifyTestOrderCommand { IdentifyNumber = identifyNumber };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.IdentifyNumber);
    }

    // --- 3. Priority Validation ---

    /// <summary>
    /// Shoulds the not have error when priority is valid.
    /// </summary>
    /// <param name="priority">The priority.</param>
    [Theory]
    [InlineData("Normal")]
    [InlineData("Urgent")]
    [InlineData("Emergency")]
    [InlineData(null)] 
    public void Should_Not_Have_Error_When_Priority_Is_Valid(string priority)
    {
        // Arrange
        var command = new ModifyTestOrderCommand { Priority = priority };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Priority);
    }

    /// <summary>
    /// Shoulds the have error when priority is invalid.
    /// </summary>
    /// <param name="priority">The priority.</param>
    [Theory]
    [InlineData("normal")] 
    [InlineData("High")]
    [InlineData("Critial")]
    [InlineData(" ")]
    public void Should_Have_Error_When_Priority_Is_Invalid(string priority)
    {
        // Arrange
        var command = new ModifyTestOrderCommand { Priority = priority };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Priority)
              .WithErrorMessage("Priority must be one of: Normal, Urgent, Emergency.");
    }

    // --- 4. Note Validation ---

    /// <summary>
    /// Shoulds the not have error when note is under 500 characters.
    /// </summary>
    [Fact]
    public void Should_Not_Have_Error_When_Note_Is_Under_500_Characters()
    {
        // Arrange
        var command = new ModifyTestOrderCommand { Note = new string('a', 499) };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Note);
    }

    /// <summary>
    /// Shoulds the not have error when note is exactly 500 characters.
    /// </summary>
    [Fact]
    public void Should_Not_Have_Error_When_Note_Is_Exactly_500_Characters()
    {
        // Arrange
        var command = new ModifyTestOrderCommand { Note = new string('a', 500) };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Note);
    }

    /// <summary>
    /// Shoulds the have error when note exceeds 500 characters.
    /// </summary>
    [Fact]
    public void Should_Have_Error_When_Note_Exceeds_500_Characters()
    {
        // Arrange
        var command = new ModifyTestOrderCommand { Note = new string('a', 501) };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Note)
              .WithErrorMessage("Note cannot exceed 500 characters.");
    }

    // --- 5. UpdatedBy Validation ---

    /// <summary>
    /// Shoulds the have error when updated by is zero.
    /// </summary>
    [Fact]
    public void Should_Have_Error_When_UpdatedBy_Is_Null()
    {
        // Arrange
        var command = new ModifyTestOrderCommand { UpdatedBy = null };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpdatedBy)
              .WithErrorMessage("UpdatedBy is required.");
    }


    /// <summary>
    /// Shoulds the not have error when updated by is valid.
    /// </summary>
    [Fact]
    public void Should_Not_Have_Error_When_UpdatedBy_Is_Valid()
    {
        // Arrange
        var command = new ModifyTestOrderCommand { UpdatedBy = "dpctor" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.UpdatedBy);
    }
}