using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation.TestHelper;
using IAM_Service.Application.Interface.IPasswordHasher;
using IAM_Service.Application.Interface.IUser;
using IAM_Service.Application.Registers.Command;
using IAM_Service.Domain.Entity;
using Moq;
using Xunit;

namespace IAM_Service.Application.UnitTests.Registers
{
    /// <summary>
    /// Unit tests for <see cref="RegistersAccountCommand"/>, 
    /// <see cref="RegistersAccountCommandHandler"/>, and <see cref="RegistersAccountCommandValidation"/>.
    /// </summary>
    public class RegistersAccountCommandTests
    {
        #region Command Validation Tests

        /// <summary>
        /// Tests that validation fails when Email is empty.
        /// </summary>
        [Fact]
        public void Validation_Should_HaveError_When_EmailIsEmpty()
        {
            var validator = new RegistersAccountCommandValidation();
            var command = new RegistersAccountCommand { Email = "" };
            var result = validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.Email);
        }

        /// <summary>
        /// Tests that validation fails when FullName contains invalid characters.
        /// </summary>
        [Fact]
        public void Validation_Should_HaveError_When_FullNameIsInvalid()
        {
            var validator = new RegistersAccountCommandValidation();
            var command = new RegistersAccountCommand { FullName = "12345" };
            var result = validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.FullName);
        }

        /// <summary>
        /// Tests that validation fails when IdentifyNumber is invalid.
        /// </summary>
        [Fact]
        public void Validation_Should_HaveError_When_IdentifyNumberIsInvalid()
        {
            var validator = new RegistersAccountCommandValidation();
            var command = new RegistersAccountCommand { IdentifyNumber = "abc" };
            var result = validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.IdentifyNumber);
        }

        /// <summary>
        /// Tests that validation fails when Password does not meet complexity requirements.
        /// </summary>
        [Fact]
        public void Validation_Should_HaveError_When_PasswordIsWeak()
        {
            var validator = new RegistersAccountCommandValidation();
            var command = new RegistersAccountCommand { Password = "123456" };
            var result = validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.Password);
        }

        #endregion

        #region CommandHandler Tests

        /// <summary>
        /// Tests that the handler successfully creates a new user
        /// when the provided email does not already exist.
        /// </summary>
        [Fact]
        public async Task Handle_Should_CreateUser_When_EmailNotExists()
        {
            // Arrange
            var mockRepo = new Mock<IUsersRepository>();
            var mockMapper = new Mock<IMapper>();
            var mockHasher = new Mock<IPasswordHasher>();

            var command = new RegistersAccountCommand
            {
                FullName = "John Doe",
                Email = "john@example.com",
                IdentifyNumber = "123456789",
                Password = "Password@123"
            };

            var userEntity = new User
            {
                FullName = command.FullName,
                Email = command.Email,
                IdentifyNumber = command.IdentifyNumber
            };

            mockRepo.Setup(r => r.GetByEmailAsync(command.Email))
                    .ReturnsAsync((User?)null);
            mockHasher.Setup(h => h.Hash(command.Password))
                      .Returns("hashed-password");
            mockMapper.Setup(m => m.Map<User>(command))
                      .Returns(userEntity);
            mockRepo.Setup(r => r.CreateUser(userEntity))
                    .Returns(Task.CompletedTask);

            var handler = new RegistersAccountCommandHandler(
                mockRepo.Object,
                mockMapper.Object,
                mockHasher.Object
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(MediatR.Unit.Value, result);
            mockRepo.Verify(r => r.GetByEmailAsync(command.Email), Times.Once);
            mockHasher.Verify(h => h.Hash(command.Password), Times.Once);
            mockMapper.Verify(m => m.Map<User>(command), Times.Once);
            mockRepo.Verify(r => r.CreateUser(userEntity), Times.Once);
            Assert.Equal("hashed-password", userEntity.Password);
        }

        /// <summary>
        /// Tests that the handler throws an <see cref="InvalidOperationException"/>
        /// when attempting to register with an email that already exists.
        /// </summary>
        [Fact]
        public async Task Handle_Should_ThrowInvalidOperationException_When_EmailExists()
        {
            // Arrange
            var mockRepo = new Mock<IUsersRepository>();
            var mockMapper = new Mock<IMapper>();
            var mockHasher = new Mock<IPasswordHasher>();

            var command = new RegistersAccountCommand
            {
                Email = "existing@example.com",
                Password = "Password@123"
            };

            mockRepo.Setup(r => r.GetByEmailAsync(command.Email))
                    .ReturnsAsync(new User { Email = command.Email });

            var handler = new RegistersAccountCommandHandler(
                mockRepo.Object,
                mockMapper.Object,
                mockHasher.Object
            );

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                handler.Handle(command, CancellationToken.None)
            );

            mockRepo.Verify(r => r.GetByEmailAsync(command.Email), Times.Once);
            mockHasher.Verify(h => h.Hash(It.IsAny<string>()), Times.Never);
            mockMapper.Verify(m => m.Map<User>(It.IsAny<RegistersAccountCommand>()), Times.Never);
        }

        #endregion
    }
}
