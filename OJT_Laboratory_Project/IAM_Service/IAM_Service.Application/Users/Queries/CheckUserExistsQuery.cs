using MediatR;

namespace IAM_Service.Application.Users.Queries
{
    /// <summary>
    /// Query to check user existence by Identifier
    /// Returns a boolean value.
    /// </summary>
    public record CheckUserExistsQuery(string IdentifyNumber) : IRequest<bool>;
}