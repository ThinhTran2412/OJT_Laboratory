using FluentValidation.TestHelper;
using IAM_Service.Application.Users.Command; 

namespace IAM_Service.Application.Tests.Users.Command
{
    /// <summary>
    /// create usecase test for CreateUserCommandValidation
    /// </summary>
    public class CreateUserCommandValidationTests
    {
        /// <summary>
        /// The validator
        /// </summary>
        private readonly CreateUserCommandValidation _validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateUserCommandValidationTests"/> class.
        /// </summary>
        public CreateUserCommandValidationTests()
        {
            _validator = new CreateUserCommandValidation();
        }

        /// <summary>
        /// Tạo một đối tượng Command hợp lệ mặc định làm cơ sở.
        /// </summary>
        /// <returns></returns>
        private CreateUserCommand GetValidCommand()
        {
            return new CreateUserCommand
            {
                Email = "valid.user@test.com",
                FullName = "Nguyễn Văn Test",
                PhoneNumber = "0901234567",
                Age = 30,
                IdentifyNumber = "123456789012", // 12 digits
                Gender = "Male",
                Address = "123 Test Street, Hanoi, Vietnam",
                DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-30).AddDays(-1))

            };
        }

        // =======================================================================
        // --- EMAIL TESTS ---
        // =======================================================================

        /// <summary>
        /// Emails the should fail when is blank.
        /// </summary>
        /// <param name="email">The email.</param>
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Email_Should_Fail_When_Is_Blank(string email)
        {
            var command = GetValidCommand();
            command.Email = email;
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage("Email cannot be blank");
        }

        /// <summary>
        /// Emails the should fail when is invalid format.
        /// </summary>
        [Fact]
        public void Email_Should_Fail_When_Is_Invalid_Format()
        {
            var command = GetValidCommand();
            command.Email = "invalid-email";
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage("Invalid email. Please enter in correct format.");
        }

        // =======================================================================
        // --- FULL NAME TESTS ---
        // =======================================================================

        /// <summary>
        /// Fulls the name should fail when invalid.
        /// </summary>
        /// <param name="fullName">The full name.</param>
        [Theory]
        [InlineData("")]
        [InlineData("Nguyen Van A123")]
        [InlineData("Nguyen! Van A")]
        public void FullName_Should_Fail_When_Invalid(string fullName)
        {
            var command = GetValidCommand();
            command.FullName = fullName;
            var result = _validator.TestValidate(command);

            if (string.IsNullOrEmpty(fullName))
                result.ShouldHaveValidationErrorFor(x => x.FullName).WithErrorMessage("Full Name cannot be blank");
            else
                result.ShouldHaveValidationErrorFor(x => x.FullName).WithErrorMessage("Full name cannot contain numbers or special characters.");
        }

        /// <summary>
        /// Fulls the name should fail when exceeds 100 characters.
        /// </summary>
        [Fact]
        public void FullName_Should_Fail_When_Exceeds_100_Characters()
        {
            var command = GetValidCommand();
            command.FullName = new string('A', 101);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.FullName)
                  .WithErrorMessage("Full name cannot exceed 100 characters.");
        }

        // =======================================================================
        // --- PHONE NUMBER TESTS ---
        // =======================================================================

        /// <summary>
        /// Phones the number should fail when invalid.
        /// </summary>
        /// <param name="phoneNumber">The phone number.</param>
        [Theory]
        [InlineData("")]
        [InlineData("12345678")]
        [InlineData("123456789012")]
        [InlineData("abcde1234")]
        public void PhoneNumber_Should_Fail_When_Invalid(string phoneNumber)
        {
            var command = GetValidCommand();
            command.PhoneNumber = phoneNumber;
            var result = _validator.TestValidate(command);

            if (string.IsNullOrEmpty(phoneNumber))
                result.ShouldHaveValidationErrorFor(x => x.PhoneNumber).WithErrorMessage("Phone Number cannot be blank");
            else
                result.ShouldHaveValidationErrorFor(x => x.PhoneNumber).WithErrorMessage("Invalid phone number. Please enter 9–11 digits.");
        }

        // =======================================================================
        // --- AGE TESTS ---
        // =======================================================================

        /// <summary>
        /// Ages the should fail when is out of range.
        /// </summary>
        /// <param name="age">The age.</param>
        [Theory]
        [InlineData(0)]
        [InlineData(101)]
        public void Age_Should_Fail_When_Is_Out_Of_Range(int age)
        {
            var command = GetValidCommand();
            command.Age = age;
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Age)
                  .WithErrorMessage("Age must be between 1 and 100.");
        }


        // =======================================================================
        // --- IDENTIFY NUMBER TESTS ---
        // =======================================================================

        /// <summary>
        /// Identifies the number should fail when invalid.
        /// </summary>
        /// <param name="id">The identifier.</param>
        [Theory]
        [InlineData("")]
        [InlineData("12345678")]
        [InlineData("1234567890")]
        [InlineData("abc123456")]
        public void IdentifyNumber_Should_Fail_When_Invalid(string id)
        {
            var command = GetValidCommand();
            command.IdentifyNumber = id;
            var result = _validator.TestValidate(command);

            if (string.IsNullOrEmpty(id))
                result.ShouldHaveValidationErrorFor(x => x.IdentifyNumber).WithErrorMessage("Identify Number cannot be blank");
            else
                result.ShouldHaveValidationErrorFor(x => x.IdentifyNumber).WithErrorMessage("Identify Number must have 9 or 12 digits.");
        }

        // =======================================================================
        // --- GENDER TESTS ---
        // =======================================================================

        /// <summary>
        /// Genders the should fail when is invalid value.
        /// </summary>
        /// <param name="gender">The gender.</param>
        [Theory]
        [InlineData("")]
        [InlineData("Other")]
        [InlineData("UNSPECIFIED")]
        public void Gender_Should_Fail_When_Is_Invalid_Value(string gender)
        {
            var command = GetValidCommand();
            command.Gender = gender;
            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Gender)
                  .WithErrorMessage("Gender can only be selected as Male or Female.");
        }

        // =======================================================================
        // --- ADDRESS TESTS ---
        // =======================================================================

        /// <summary>
        /// Addresses the should fail when is too short.
        /// </summary>
        /// <param name="address">The address.</param>
        [Theory]
        [InlineData("")]
        [InlineData("A")]
        public void Address_Should_Fail_When_Is_Too_Short(string address)
        {
            var command = GetValidCommand();
            command.Address = address;
            var result = _validator.TestValidate(command);

            if (string.IsNullOrEmpty(address))
                result.ShouldHaveValidationErrorFor(x => x.Address).WithErrorMessage("Address cannot be blank");
            else
                result.ShouldHaveValidationErrorFor(x => x.Address).WithErrorMessage("Address must be at least 5 characters long.");
        }

        /// <summary>
        /// Addresses the should fail when is too long.
        /// </summary>
        [Fact]
        public void Address_Should_Fail_When_Is_Too_Long()
        {
            var command = GetValidCommand();
            command.Address = new string('X', 256);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Address)
                  .WithErrorMessage("The address cannot exceed 255 characters.");
        }

        // =======================================================================
        // --- DATE OF BIRTH TESTS ---
        // =======================================================================

        /// <summary>
        /// Dates the of birth should fail when is future.
        /// </summary>
        [Fact]
        public void DateOfBirth_Should_Fail_When_Is_Future()
        {
            var command = GetValidCommand();
            command.DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.DateOfBirth)
                  .WithErrorMessage("Date of birth cannot exceed the current date.");
        }

        /// <summary>
        /// Dates the of birth should fail when is too old.
        /// </summary>
        [Fact]
        public void DateOfBirth_Should_Fail_When_Is_Too_Old()
        {
            var command = GetValidCommand();
            command.DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-100).AddDays(-1));
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.DateOfBirth)
                  .WithErrorMessage("Invalid date of birth (age too old).");
        }

        // =======================================================================
        // --- HAPPY PATH (Covering all success) ---
        // =======================================================================

        /// <summary>
        /// Shoulds the pass validation when all properties are valid.
        /// </summary>
        [Fact]
        public void Should_Pass_Validation_When_All_Properties_Are_Valid()
        {
            // Arrange: 
            var command = GetValidCommand();

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}