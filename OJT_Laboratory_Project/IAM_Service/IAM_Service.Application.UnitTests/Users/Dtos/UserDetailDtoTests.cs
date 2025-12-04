using IAM_Service.Application.DTOs.User;
using Xunit;

namespace IAM_Service.Application.UnitTests.Users.DTOs
{
    public class UserDetailDtoTests
    {
        [Fact]
        public void CanGetAndSetProperties()
        {
            // Arrange
            var dto = new UserDetailDto();

            // Act
            dto.UserId = 1;
            dto.FullName = "John Doe";
            dto.Email = "john@example.com";
            dto.PhoneNumber = "123456789";
            dto.IdentifyNumber = "ID123";
            dto.Gender = "Male";
            dto.Age = 30;
            dto.Address = "123 Street";
            dto.DateOfBirth = new DateOnly(1993, 5, 5);
            dto.RoleId = 2;
            dto.RoleName = "Admin";
            dto.RoleCode = "ADM";
            dto.Privileges.Add("Read");
            dto.Privileges.Add("Write");

            // Assert
            Assert.Equal(1, dto.UserId);
            Assert.Equal("John Doe", dto.FullName);
            Assert.Equal("john@example.com", dto.Email);
            Assert.Equal("123456789", dto.PhoneNumber);
            Assert.Equal("ID123", dto.IdentifyNumber);
            Assert.Equal("Male", dto.Gender);
            Assert.Equal(30, dto.Age);
            Assert.Equal("123 Street", dto.Address);
            Assert.Equal(new DateOnly(1993, 5, 5), dto.DateOfBirth);
            Assert.Equal(2, dto.RoleId);
            Assert.Equal("Admin", dto.RoleName);
            Assert.Equal("ADM", dto.RoleCode);
            Assert.Contains("Read", dto.Privileges);
            Assert.Contains("Write", dto.Privileges);
        }
    }
}
