using System;
using IAM_Service.Application.Common.Exceptions;
using Xunit;

namespace IAM_Service.Application.UnitTests.Exceptions
{
    /// <summary>
    /// Unit tests for <see cref="InvalidOrExpiredTokenException"/>.
    /// </summary>
    public class InvalidOrExpiredTokenExceptionTests
    {
        /// <summary>
        /// Tests that the constructor correctly sets the <see cref="Exception.Message"/> property.
        /// </summary>
        [Fact]
        public void Constructor_Sets_Message()
        {
            // Arrange
            var message = "Token is invalid or expired";

            // Act
            var ex = new InvalidOrExpiredTokenException(message);

            // Assert
            Assert.Equal(message, ex.Message);
        }

        /// <summary>
        /// Tests that <see cref="InvalidOrExpiredTokenException"/> inherits from <see cref="System.Exception"/>.
        /// </summary>
        [Fact]
        public void Constructor_Inherits_Exception()
        {
            // Arrange & Act
            var ex = new InvalidOrExpiredTokenException("Test");

            // Assert
            Assert.IsType<InvalidOrExpiredTokenException>(ex);
            Assert.IsAssignableFrom<Exception>(ex);
        }
    }
}
