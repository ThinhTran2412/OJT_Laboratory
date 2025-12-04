using IAM_Service.Application.Interface.IRole;
using IAM_Service.Domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace IAM_Service.Application.Roles.Command
{
    /// <summary>
    /// Handles the deletion of a role.
    /// </summary>
    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, bool>
    {
        private readonly IRoleCommandRepository _roleCommandRepository;
        private readonly ILogger<DeleteRoleCommandHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteRoleCommandHandler"/> class.
        /// </summary>
        /// <param name="roleCommandRepository">The role command repository.</param>
        /// <param name="logger">The logger.</param>
        public DeleteRoleCommandHandler(
            IRoleCommandRepository roleCommandRepository,
            ILogger<DeleteRoleCommandHandler> logger)
        {
            _roleCommandRepository = roleCommandRepository;
            _logger = logger;
        }

        /// <summary>
        /// Handles the role deletion request.
        /// </summary>
        /// <param name="request">The delete role command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>True if the role was deleted, false if it was not found.</returns>
        public async Task<bool> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting role with ID {RoleId}", request.RoleId);

            try
            {
                bool isDeleted = await _roleCommandRepository.DeleteAsync(request.RoleId, cancellationToken);
                
                if (isDeleted)
                {
                    _logger.LogInformation("Successfully deleted role with ID {RoleId}", request.RoleId);
                }
                else
                {
                    _logger.LogWarning("Attempted to delete non-existent role with ID {RoleId}", request.RoleId);
                }

                return isDeleted;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role with ID {RoleId}", request.RoleId);
                throw;
            }
        }
    }
}
