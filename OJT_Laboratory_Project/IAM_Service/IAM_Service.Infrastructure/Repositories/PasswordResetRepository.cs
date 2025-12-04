using IAM_Service.Application.Interface.IPasswordResetRepository;
using IAM_Service.Domain.Entity;
using IAM_Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IAM_Service.Infrastructure.Repositories
{
    /// <summary>
    /// Create PasswordResetRepository
    /// </summary>
    /// <seealso cref="IAM_Service.Application.Interface.IPasswordResetRepository.IPasswordResetRepository" />
    public class PasswordResetRepository : IPasswordResetRepository
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly AppDbContext _context;
        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordResetRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public PasswordResetRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds the asynchronous.
        /// </summary>
        /// <param name="passwordReset">The password reset.</param>
        public async Task AddAsync(PasswordReset passwordReset)
        {
            await _context.PasswordResets.AddAsync(passwordReset);
        }

        /// <summary>
        /// Gets the by token asynchronous.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public async Task<PasswordReset?> GetByTokenAsync(string token)
        {
            return await _context.PasswordResets.FirstOrDefaultAsync(x => x.Token == token);
        }
        /// <summary>
        /// Mark a password reset token as used.
        /// </summary>
        public async Task MarkUsedAsync(PasswordReset passwordReset)
        {
            passwordReset.IsUsed = true;
            _context.PasswordResets.Update(passwordReset);
            await _context.SaveChangesAsync();
        }
        /// <summary>
        /// Saves the changes asynchronous.
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
