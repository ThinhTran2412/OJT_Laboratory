using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace SharedLibrary.DependencyInjection
{
    /// <summary>
    ///  Register JWT (Bearer Authentication) mechanism
    ///  This extension function will:
    ///  Read JWT configuration (SecretKey, Issuer, Audience) from appsettings.json
    ///  Configure authentication according to JWT Bearer standard
    ///  Verify the token sent in the Authorization header every time the API is called
    /// </summary>
    public static class JWTAuthenticationScheme
    {
        /// <summary>
        /// Adds the JWT authentication scheme.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">key - JWT SecretKey cannot be null or empty.</exception>
        public static IServiceCollection AddJWTAuthenticationScheme(this IServiceCollection services, IConfiguration config)
        {
            // Lấy thông tin JWT từ appsettings.json
            var section = config.GetSection("JwtOptions");
            var key = section.GetValue<string>("SecretKey");
            var issuer = section.GetValue<string>("Issuer");
            var audience = section.GetValue<string>("Audience");

            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key), "JWT SecretKey cannot be null or empty.");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),

                        ClockSkew = TimeSpan.Zero
                    };
                });
            Console.WriteLine($"Loaded JWT SecretKey: {key ?? "NULL"}");

            services.AddAuthorization();
            return services;
        }
    }
}
