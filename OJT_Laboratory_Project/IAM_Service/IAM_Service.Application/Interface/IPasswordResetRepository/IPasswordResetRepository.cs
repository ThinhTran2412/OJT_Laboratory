using IAM_Service.Domain.Entity;

namespace IAM_Service.Application.Interface.IPasswordResetRepository
{
    public interface IPasswordResetRepository
    {
        Task AddAsync(PasswordReset passwordReset);
        Task<PasswordReset?> GetByTokenAsync(string token);
        Task SaveChangesAsync();
        Task MarkUsedAsync(PasswordReset passwordReset);
    }
}
