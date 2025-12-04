using FluentValidation;

namespace IAM_Service.Application.Roles.Command
{
    /// <summary>
    /// Validator for the UpdateRoleCommand.
    /// </summary>
    public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateRoleCommandValidator"/> class.
        /// </summary>
        public UpdateRoleCommandValidator()
        {
            RuleFor(x => x.RoleId)
                .GreaterThan(0).WithMessage("Role ID must be greater than 0.");

            RuleFor(x => x.Dto.Name)
                .NotEmpty().WithMessage("Role name is required.")
                .MaximumLength(100).WithMessage("Name cannot be longer than 100 characters.");

            RuleFor(x => x.Dto.Description)
                .MaximumLength(500).WithMessage("Description cannot be longer than 500 characters.");

            RuleForEach(x => x.Dto.PrivilegeIds)
                .GreaterThan(0).WithMessage("Privilege ID must be greater than 0.")
                .When(x => x.Dto.PrivilegeIds != null);
        }
    }
}
