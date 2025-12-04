using IAM_Service.Application.Interface.IJwt;
using IAM_Service.Application.Interface.IPrivilege;
using IAM_Service.Domain.Entity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

/// <summary>
/// JWT token provider implementation.
/// </summary>
namespace IAM_Service.Infrastructure.Authentication
{
    /// <summary>
    /// Provides methods for generating JWT tokens.
    /// </summary>
    internal sealed class JwtProvider : IJwtProvider
    {
        /// <summary>
        /// The JWT options for token generation.
        /// </summary>
        private readonly JwtOptions _options;
        private readonly IPrivilegeRepository _privilegeRepository;
        /// <summary>
        /// Constructor for the JwtProvider.
        /// </summary> <param name="options">The JWT options for token generation.</param
        public JwtProvider(IOptions<JwtOptions> options, IPrivilegeRepository privilegeRepository)
        {
            _options = options.Value;
            _privilegeRepository = privilegeRepository;
        }
        /// <summary>
        /// Generates a JWT token for the specified user.
        /// </summary>  <param name="user">The user for whom to generate the token.</param>
        /// <returns>A JWT token as a string.</returns>
        public async Task<string> Generate(User user)
        {
            JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();
            // Get privileges from both RoleId and UserId
            var rolePrivileges = await _privilegeRepository.GetPrivilegeNamesByRoleIdAsync(user.RoleId) ?? new List<string>();
            var userPrivileges = await _privilegeRepository.GetPrivilegeNamesByUserIdAsync(user.UserId) ?? new List<string>();

            // Combine both lists and remove duplicates
            var privilegeNames = rolePrivileges.Union(userPrivileges).ToList();
            // Define token claims
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("identifyNumber", user.IdentifyNumber ?? ""),
                new Claim("role", user.RoleId.ToString()),
                new Claim(ClaimTypes.Name, user.FullName)

            };

            foreach (var privilegeName in privilegeNames)
            {
                claims.Add(new Claim("privilege", privilegeName));
            }

            // Create a symmetric security key from the configured secret
            var signingKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_options.SecretKey)
            );

            // Define signing credentials using HMAC-SHA256
            var signingCredentials = new SigningCredentials(
                signingKey,
                SecurityAlgorithms.HmacSha256
            );

            // Create the JWT token
            var tokenDescriptor = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_options.ExpirationMinutes),
            signingCredentials: signingCredentials
            );

            // Serialize token to string
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public string GenerateForService(string clientId, string scope)
        {
            // Token service chỉ chứa các claims liên quan đến Service
            var claims = new List<Claim>
        {
        new Claim("client_id", clientId),
        new Claim("scope", scope)
        };

            var signingKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_options.SecretKey)
            );

            var signingCredentials = new SigningCredentials(
                signingKey,
                SecurityAlgorithms.HmacSha256
            );

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15), // Thời gian sống ngắn cho Service Token (15 phút)
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
