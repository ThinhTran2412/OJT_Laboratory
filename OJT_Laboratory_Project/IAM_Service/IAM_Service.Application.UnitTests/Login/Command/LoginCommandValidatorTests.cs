using FluentValidation.TestHelper;
using IAM_Service.Application.Login;

namespace IAM_Service.Application.UnitTests.Login.Command
{
    /// <summary>
    /// create usecase test for LoginCommandValidator
    /// </summary>
    public class LoginCommandValidatorTests
    {
        /// <summary>
        /// The validator
        /// </summary>
        private readonly LoginCommandValidator _validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginCommandValidatorTests"/> class.
        /// </summary>
        public LoginCommandValidatorTests()
        {
            _validator = new LoginCommandValidator();
        }

        // ----------------- Test Case for Email -----------------

        /// <summary>
        /// Shoulds the have error when email is empty.
        /// </summary>
        [Fact]
        public void Should_Have_Error_When_Email_Is_Empty()
        {
            var command = new LoginCommand { Email = "", Password = "AnyPassword123" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Email)
                .WithErrorMessage("'Email' must not be empty.");
        }

        /// <summary>
        /// Shoulds the have error when email is not a valid address.
        /// </summary>
        [Fact]
        public void Should_Have_Error_When_Email_Is_Not_A_Valid_Address()
        {
            var command = new LoginCommand { Email = "not-an-email", Password = "AnyPassword123" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Email)
                .WithErrorMessage("'Email' is not a valid email address.");
        }

        /// <summary>
        /// Shoulds the not have error when email is valid.
        /// </summary>
        [Fact]
        public void Should_Not_Have_Error_When_Email_Is_Valid()
        {
            var command = new LoginCommand { Email = "valid.user@example.com", Password = "AnyPassword123" };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(x => x.Email);
        }

        // ----------------- Test Case for Password -----------------

        /// <summary>
        /// Shoulds the have error when password is empty.
        /// </summary>
        [Fact]
        public void Should_Have_Error_When_Password_Is_Empty()
        {
            var command = new LoginCommand { Email = "valid@example.com", Password = "" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage("Password not null");
        }

        /// <summary>
        /// Shoulds the not have error when password is provided.
        /// </summary>
        [Fact]
        public void Should_Not_Have_Error_When_Password_Is_Provided()
        {
            var command = new LoginCommand { Email = "valid@example.com", Password = "SecurePassword123" };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(x => x.Password);
        }

        // ----------------- Summary Test Case -----------------

        /// <summary>
        /// Shoulds the have multiple errors when both fields are invalid.
        /// </summary>
        [Fact]
        public void Should_Have_Multiple_Errors_When_Both_Fields_Are_Invalid()
        {
            var command = new LoginCommand { Email = "bademail", Password = "" };
            var result = _validator.TestValidate(command);

            Assert.False(result.IsValid);
            Assert.Equal(2, result.Errors.Count);

            result.ShouldHaveValidationErrorFor(x => x.Email);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }
    }
}