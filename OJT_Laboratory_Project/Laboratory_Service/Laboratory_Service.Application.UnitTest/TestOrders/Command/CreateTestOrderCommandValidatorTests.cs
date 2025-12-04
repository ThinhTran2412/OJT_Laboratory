//using FluentValidation.TestHelper;
//using Laboratory_Service.Application.TestOrders.Commands;

///// <summary>
///// Create use case test for CreateTestOrderCommandValidator
///// </summary>
//public class CreateTestOrderCommandValidatorTests
//{
//    /// <summary>
//    /// The validator
//    /// </summary>
//    private readonly CreateTestOrderCommandValidator _validator;

//    /// <summary>
//    /// The valid command
//    /// </summary>
//    private readonly CreateTestOrderCommand _validCommand = new CreateTestOrderCommand
//    {
//        IdentifyNumber = "123456789",
//        ServicePackageId = Guid.NewGuid(),
//        Priority = "Normal",
//        Note = "Standard notes.",
//        CreatedBy = 100
//    };

//    /// <summary>
//    /// Initializes a new instance of the <see cref="CreateTestOrderCommandValidatorTests"/> class.
//    /// </summary>
//    public CreateTestOrderCommandValidatorTests()
//    {
//        _validator = new CreateTestOrderCommandValidator();
//    }

//    /// <summary>
//    /// Helper function to create a new command instance based on the valid base command,
//    /// allowing a single property to be modified for testing failure cases.
//    /// This avoids the 'with' syntax since CreateTestOrderCommand is a class.
//    /// </summary>
//    /// <param name="modifier">The modifier.</param>
//    /// <returns></returns>
//    private CreateTestOrderCommand CreateCommand(Action<CreateTestOrderCommand> modifier)
//    {
//        var command = new CreateTestOrderCommand
//        {
//            IdentifyNumber = _validCommand.IdentifyNumber,
//            ServicePackageId = _validCommand.ServicePackageId,
//            Priority = _validCommand.Priority,
//            Note = _validCommand.Note,
//            CreatedBy = _validCommand.CreatedBy
//        };
//        modifier(command);
//        return command;
//    }

//    // --- 1. Test: Valid Command (Happy Path) ---

//    /// <summary>
//    /// Shoulds the not have validation errors when command is valid.
//    /// </summary>
//    [Fact]
//    public void Should_Not_Have_Validation_Errors_When_Command_Is_Valid()
//    {
//        // Act
//        var result = _validator.TestValidate(_validCommand);

//        // Assert
//        result.ShouldNotHaveAnyValidationErrors();
//    }

//    // --- 2. Test: IdentifyNumber Field Validation ---

//    /// <summary>
//    /// Shoulds the have error when identify number is empty.
//    /// </summary>
//    /// <param name="identifyNumber">The identify number.</param>
//    [Theory]
//    [InlineData(null)]
//    [InlineData("")]
//    public void Should_Have_Error_When_IdentifyNumber_Is_Empty(string identifyNumber)
//    {
//        // Arrange
//        var command = CreateCommand(c => c.IdentifyNumber = identifyNumber);

//        // Act & Assert
//        _validator.TestValidate(command)
//            .ShouldHaveValidationErrorFor(x => x.IdentifyNumber)
//            .WithErrorMessage("IdentifyNumber is required.");
//    }

//    /// <summary>
//    /// Shoulds the length of the have error when identify number has invalid format or.
//    /// </summary>
//    /// <param name="identifyNumber">The identify number.</param>
//    [Theory]
//    [InlineData("12345678")]
//    [InlineData("ABC123456")]
//    public void Should_Have_Error_When_IdentifyNumber_Has_Invalid_Format_Or_Length(string identifyNumber)
//    {
//        // Arrange
//        var command = CreateCommand(c => c.IdentifyNumber = identifyNumber);

//        // Act & Assert
//        _validator.TestValidate(command)
//            .ShouldHaveValidationErrorFor(x => x.IdentifyNumber)
//            .WithErrorMessage("IdentifyNumber must contain either 9 or 12 digits only.");
//    }

//    /// <summary>
//    /// Shoulds the length of the not have error when identify number has valid.
//    /// </summary>
//    /// <param name="identifyNumber">The identify number.</param>
//    [Theory]
//    [InlineData("123456789")]
//    [InlineData("123456789012")]
//    public void Should_Not_Have_Error_When_IdentifyNumber_Has_Valid_Length(string identifyNumber)
//    {
//        // Arrange
//        var command = CreateCommand(c => c.IdentifyNumber = identifyNumber);

//        // Act & Assert
//        _validator.TestValidate(command)
//            .ShouldNotHaveValidationErrorFor(x => x.IdentifyNumber);
//    }

//    // --- 3. Test: ServicePackageId Field Validation ---

//    /// <summary>
//    /// Shoulds the have error when service package identifier is null.
//    /// </summary>
//    [Fact]
//    public void Should_Have_Error_When_ServicePackageId_Is_Null()
//    {
//        // Arrange
//        var command = CreateCommand(c => c.ServicePackageId = null);

//        // Act & Assert
//        _validator.TestValidate(command)
//            .ShouldHaveValidationErrorFor(x => x.ServicePackageId)
//            .WithErrorMessage("ServicePackageId is required.");
//    }

//    // --- 4. Test: Priority Field Validation ---

//    /// <summary>
//    /// Shoulds the have error when priority is empty.
//    /// </summary>
//    /// <param name="priority">The priority.</param>
//    [Theory]
//    [InlineData(null)]
//    [InlineData("")]
//    public void Should_Have_Error_When_Priority_Is_Empty(string priority)
//    {
//        // Arrange
//        var command = CreateCommand(c => c.Priority = priority);

//        // Act & Assert
//        _validator.TestValidate(command)
//            .ShouldHaveValidationErrorFor(x => x.Priority)
//            .WithErrorMessage("Priority is required.");
//    }

//    /// <summary>
//    /// Shoulds the have error when priority is not a valid value.
//    /// </summary>
//    /// <param name="priority">The priority.</param>
//    [Theory]
//    [InlineData("high")]
//    public void Should_Have_Error_When_Priority_Is_Not_A_Valid_Value(string priority)
//    {
//        // Arrange
//        var command = CreateCommand(c => c.Priority = priority);

//        // Act & Assert
//        _validator.TestValidate(command)
//            .ShouldHaveValidationErrorFor(x => x.Priority)
//            .WithErrorMessage("Priority must be one of: Normal, Urgent, Emergency.");
//    }

//    // --- 5. Test: Note Field Validation ---

//    /// <summary>
//    /// Shoulds the have error when note exceeds 500 characters.
//    /// </summary>
//    [Fact]
//    public void Should_Have_Error_When_Note_Exceeds_500_Characters()
//    {
//        // Arrange
//        var longNote = new string('A', 501);
//        var command = CreateCommand(c => c.Note = longNote);

//        // Act & Assert
//        _validator.TestValidate(command)
//            .ShouldHaveValidationErrorFor(x => x.Note)
//            .WithErrorMessage("Note cannot exceed 500 characters.");
//    }

//    /// <summary>
//    /// Shoulds the not have error when note is null.
//    /// </summary>
//    [Fact]
//    public void Should_Not_Have_Error_When_Note_Is_Null()
//    {
//        // Arrange
//        var command = CreateCommand(c => c.Note = null);

//        // Act & Assert
//        _validator.TestValidate(command)
//            .ShouldNotHaveValidationErrorFor(x => x.Note);
//    }

//    // --- 6. Test: CreatedBy Field Validation ---

//    /// <summary>
//    /// Shoulds the have error when created by is zero.
//    /// </summary>
//    [Fact]
//    public void Should_Have_Error_When_CreatedBy_Is_Zero()
//    {
//        // Arrange
//        var command = CreateCommand(c => c.CreatedBy = 0);

//        // Act & Assert
//        _validator.TestValidate(command)
//            .ShouldHaveValidationErrorFor(x => x.CreatedBy)
//            .WithErrorMessage("CreatedBy is required.");
//    }
//}