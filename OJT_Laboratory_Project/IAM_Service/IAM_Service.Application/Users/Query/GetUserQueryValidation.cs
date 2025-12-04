using FluentValidation;

namespace IAM_Service.Application.Users.Query
{
    /// <summary>
    /// Validation rules for GetUserQuery to ensure query params are valid
    /// </summary>
    public class GetUserQueryValidation : AbstractValidator<GetUserQuery>
    {
        public GetUserQueryValidation()
        {
            // Age range validation
            RuleFor(x => x.MinAge)
                .GreaterThanOrEqualTo(0).When(x => x.MinAge.HasValue)
                .WithMessage("MinAge must be greater than or equal to 0");

            RuleFor(x => x.MaxAge)
                .GreaterThanOrEqualTo(0).When(x => x.MaxAge.HasValue)
                .WithMessage("MaxAge must be greater than or equal to 0");

            RuleFor(x => x)
                .Must(q => !(q.MinAge.HasValue && q.MaxAge.HasValue) || q.MinAge < q.MaxAge)
                .WithMessage("MinAge must be less than MaxAge");
        }
    }
}


