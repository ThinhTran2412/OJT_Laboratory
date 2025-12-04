using FluentValidation.TestHelper;
using IAM_Service.Application.DTOs.Roles;
using IAM_Service.Application.Roles.Command;
using Xunit;

namespace IAM_Service.Application.UnitTests.Roles.Command
{
    /// <summary>
    /// Unit tests for <see cref="CreateRoleCommandValidator"/>.
    /// </summary>
    public class CreateRoleCommandValidatorTests
    {
        /// <summary>
        /// Validator instance to test <see cref="CreateRoleCommand"/>.
        /// </summary>
        private readonly CreateRoleCommandValidator _validator;

        /// <summary>
        /// Initializes a new instance of <see cref="CreateRoleCommandValidatorTests"/>.
        /// </summary>
        public CreateRoleCommandValidatorTests()
        {
            _validator = new CreateRoleCommandValidator();
        }

        /// <summary>
        /// Tests that validation fails when the role name is empty.
        /// </summary>
        [Fact]
        public void Should_HaveError_When_NameIsEmpty()
        {
            var command = new CreateRoleCommand(new RoleCreateDto { Name = "", Code = "CODE" });
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.Dto.Name);
        }

        /// <summary>
        /// Tests that validation fails when the role code is empty.
        /// </summary>
        [Fact]
        public void Should_HaveError_When_CodeIsEmpty()
        {
            var command = new CreateRoleCommand(new RoleCreateDto { Name = "Role", Code = "" });
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.Dto.Code);
        }

        /// <summary>
        /// Tests that validation fails when the privilege IDs are invalid (zero or negative).
        /// </summary>
        [Fact]
        public void Should_HaveError_When_PrivilegeIdsInvalid()
        {
            var command = new CreateRoleCommand(new RoleCreateDto
            {
                Name = "Role",
                Code = "CODE",
                PrivilegeIds = new System.Collections.Generic.List<int> { 0, -1 }
            });
            var result = _validator.TestValidate(command);

            // Validate each invalid privilege ID separately
            foreach (var i in command.Dto.PrivilegeIds)
            {
                result.ShouldHaveValidationErrorFor(c => c.Dto.PrivilegeIds[i]);
            }
        }

        /// <summary>
        /// Tests that validation passes for a valid role DTO with proper name, code, and privileges.
        /// </summary>
        [Fact]
        public void Should_NotHaveError_When_Valid()
        {
            var command = new CreateRoleCommand(new RoleCreateDto
            {
                Name = "Role",
                Code = "CODE",
                PrivilegeIds = new System.Collections.Generic.List<int> { 1, 2 }
            });
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
