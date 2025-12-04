using FluentValidation.TestHelper;
using IAM_Service.Application.Users.Command;

namespace IAM_Service.Application.Tests.Users.Command
{
    /// <summary>
    /// 
    /// </summary>
    public class UpdateUserCommandValidationTests
    {
        /// <summary>
        /// The validator
        /// </summary>
        private readonly UpdateUserCommandValidation _validator = new();

        // -------------------------------
        // UserId
        // -------------------------------
        /// <summary>
        /// Shoulds the fail when user identifier is invalid.
        /// </summary>
        [Fact]
        public void Should_Fail_When_UserId_Is_Invalid()
        {
            var command = new UpdateUserCommand { UserId = 0 };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.UserId);
        }

        // -------------------------------
        // FullName
        // -------------------------------
        /// <summary>
        /// Shoulds the fail when full name has invalid characters.
        /// </summary>
        [Fact]
        public void Should_Fail_When_FullName_Has_Invalid_Characters()
        {
            var command = new UpdateUserCommand
            {
                UserId = 1,
                FullName = "John123!"
            };

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.FullName);
        }

        /// <summary>
        /// Shoulds the fail when full name too long.
        /// </summary>
        [Fact]
        public void Should_Fail_When_FullName_Too_Long()
        {
            var command = new UpdateUserCommand
            {
                UserId = 1,
                FullName = new string('a', 101)
            };

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.FullName);
        }

        // -------------------------------
        // Email
        // -------------------------------
        /// <summary>
        /// Shoulds the fail when email invalid format.
        /// </summary>
        [Fact]
        public void Should_Fail_When_Email_Invalid_Format()
        {
            var command = new UpdateUserCommand
            {
                UserId = 1,
                Email = "not-an-email"
            };

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        /// <summary>
        /// Shoulds the fail when email is empty.
        /// </summary>
        [Fact]
        public void Should_Fail_When_Email_Is_Empty()
        {
            var command = new UpdateUserCommand
            {
                UserId = 1,
                Email = ""
            };

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        // -------------------------------
        // PhoneNumber
        // -------------------------------
        /// <summary>
        /// Shoulds the fail invalid phone.
        /// </summary>
        [Fact]
        public void Should_Fail_Invalid_Phone()
        {
            var command = new UpdateUserCommand { UserId = 1, PhoneNumber = "abc123" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
        }

        /// <summary>
        /// Shoulds the fail when phone too short.
        /// </summary>
        [Fact]
        public void Should_Fail_When_Phone_Too_Short()
        {
            var command = new UpdateUserCommand { UserId = 1, PhoneNumber = "12345" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
        }

        /// <summary>
        /// Shoulds the fail when phone too long.
        /// </summary>
        [Fact]
        public void Should_Fail_When_Phone_Too_Long()
        {
            var command = new UpdateUserCommand { UserId = 1, PhoneNumber = "012345678901234" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
        }

        // -------------------------------
        // Age
        // -------------------------------
        /// <summary>
        /// Shoulds the fail when age out of range.
        /// </summary>
        [Fact]
        public void Should_Fail_When_Age_Out_Of_Range()
        {
            var command1 = new UpdateUserCommand { UserId = 1, Age = 0 };
            var command2 = new UpdateUserCommand { UserId = 1, Age = 150 };

            var r1 = _validator.TestValidate(command1);
            var r2 = _validator.TestValidate(command2);

            r1.ShouldHaveValidationErrorFor(x => x.Age);
            r2.ShouldHaveValidationErrorFor(x => x.Age);
        }

        // -------------------------------
        // Gender
        // -------------------------------
        /// <summary>
        /// Shoulds the fail when gender invalid.
        /// </summary>
        [Fact]
        public void Should_Fail_When_Gender_Invalid()
        {
            var command = new UpdateUserCommand
            {
                UserId = 1,
                Gender = "Unknown"
            };

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Gender);
        }

        // -------------------------------
        // Address
        // -------------------------------
        /// <summary>
        /// Shoulds the fail when address too short.
        /// </summary>
        [Fact]
        public void Should_Fail_When_Address_Too_Short()
        {
            var command = new UpdateUserCommand
            {
                UserId = 1,
                Address = "abc"
            };

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Address);
        }

        /// <summary>
        /// Shoulds the fail when address too long.
        /// </summary>
        [Fact]
        public void Should_Fail_When_Address_Too_Long()
        {
            var command = new UpdateUserCommand
            {
                UserId = 1,
                Address = new string('a', 256)
            };

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Address);
        }

        // -------------------------------
        // DateOfBirth
        // -------------------------------
        /// <summary>
        /// Shoulds the fail invalid dob.
        /// </summary>
        [Fact]
        public void Should_Fail_Invalid_DOB()
        {
            var command = new UpdateUserCommand
            {
                UserId = 1,
                DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddDays(1)) // future date
            };

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.DateOfBirth);
        }

        /// <summary>
        /// Shoulds the fail when dob too old.
        /// </summary>
        [Fact]
        public void Should_Fail_When_DOB_Too_Old()
        {
            var command = new UpdateUserCommand
            {
                UserId = 1,
                DateOfBirth = new DateOnly(1800, 1, 1)
            };

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.DateOfBirth);
        }

        // -------------------------------
        // PrivilegeIds
        // -------------------------------
        /// <summary>
        /// Shoulds the fail when privilege ids contain invalid identifier.
        /// </summary>
        [Fact]
        public void Should_Fail_When_PrivilegeIds_Contain_Invalid_Id()
        {
            var command = new UpdateUserCommand
            {
                UserId = 1,
                PrivilegeIds = new List<int> { 1, 0, -5 }
            };

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor("PrivilegeIds[1]");
            result.ShouldHaveValidationErrorFor("PrivilegeIds[2]");
        }

        // -------------------------------
        // Valid case
        // -------------------------------
        /// <summary>
        /// Shoulds the pass when valid command.
        /// </summary>
        [Fact]
        public void Should_Pass_When_Valid_Command()
        {
            var command = new UpdateUserCommand
            {
                UserId = 1,
                FullName = "John Doe",
                PhoneNumber = "0123456789",
                Gender = "Male",
                Age = 25,
                Address = "HCM City",
                DateOfBirth = new DateOnly(2000, 1, 1),
                Email = "test@test.com",
                PrivilegeIds = new List<int> { 1, 2 }
            };

            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        // -------------------------------
        // Skip validation when null
        // -------------------------------
        /// <summary>
        /// Shoulds the skip validation when fields are null.
        /// </summary>
        [Fact]
        public void Should_Skip_Validation_When_Fields_Are_Null()
        {
            var command = new UpdateUserCommand { UserId = 1 };

            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(x => x.FullName);
            result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
            result.ShouldNotHaveValidationErrorFor(x => x.Gender);
        }
    }
}
