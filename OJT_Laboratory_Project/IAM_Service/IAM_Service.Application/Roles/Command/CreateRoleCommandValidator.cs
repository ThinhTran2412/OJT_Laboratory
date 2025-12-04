using FluentValidation;

namespace IAM_Service.Application.Roles.Command
{
    /// <summary>
    /// Create validation CreateRoleCommand
    /// </summary>
    /// <seealso cref="FluentValidation.AbstractValidator&lt;IAM_Service.Application.Roles.Command.CreateRoleCommand&gt;" />
    public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateRoleCommandValidator"/> class.
        /// </summary>
        public CreateRoleCommandValidator()
        {
            RuleFor(x => x.Dto.Name)
                .NotEmpty().WithMessage("Role name is required.");

            RuleFor(x => x.Dto.Code)
                .NotEmpty().WithMessage("Role code is required.");

            RuleForEach(x => x.Dto.PrivilegeIds)
                .GreaterThan(0)
                .When(x => x.Dto.PrivilegeIds != null && x.Dto.PrivilegeIds.Count > 0);
        }
    }
}
