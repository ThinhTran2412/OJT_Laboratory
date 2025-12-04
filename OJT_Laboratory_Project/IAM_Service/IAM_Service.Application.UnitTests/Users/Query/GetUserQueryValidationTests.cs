using FluentValidation.TestHelper;
using IAM_Service.Application.Users.Query;

namespace IAM_Service.Application.UnitTests.Users.Query
{
    /// <summary>
    /// 
    /// </summary>
    public class GetUserQueryValidationTests
    {
        /// <summary>
        /// The validator
        /// </summary>
        private readonly GetUserQueryValidation _validator = new();
        /// <summary>
        /// Shoulds the have error when minimum age greater than maximum age.
        /// </summary>
        [Fact]
        public void Should_Have_Error_When_MinAge_GreaterThan_MaxAge()
        {
            var query = new GetUserQuery { MinAge = 40, MaxAge = 20 };
            var result = _validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x)
                .WithErrorMessage("MinAge must be less than MaxAge");
        }
    }
}
