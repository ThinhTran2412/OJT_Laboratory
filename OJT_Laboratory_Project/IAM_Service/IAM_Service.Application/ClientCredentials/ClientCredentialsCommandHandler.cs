using MediatR;
using IAM_Service.Application.Interface.IJwt;
using IAM_Service.Application.Interface.IClient;

namespace IAM_Service.Application.ClientCredentials
{
    /// <summary>
    /// Handles the <see cref="ClientCredentialsCommand"/> to perform OAuth2 client credentials flow.
    /// </summary>
    public class ClientCredentialsCommandHandler : IRequestHandler<ClientCredentialsCommand, object>
    {
        private readonly IJwtProvider _jwtProvider;
        private readonly IClientRepository _clientRepository;

        /// <summary>
        /// Initializes a new instance of <see cref="ClientCredentialsCommandHandler"/>.
        /// </summary>
        /// <param name="jwtProvider">JWT provider to generate access tokens.</param>
        /// <param name="clientRepository">Repository to validate client credentials.</param>
        public ClientCredentialsCommandHandler(IJwtProvider jwtProvider, IClientRepository clientRepository)
        {
            _jwtProvider = jwtProvider;
            _clientRepository = clientRepository;
        }

        /// <summary>
        /// Handles the client credentials command by validating the client and generating an access token.
        /// </summary>
        /// <param name="request">The client credentials command.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        /// Returns an anonymous object containing the access token, token type, and expiration.
        /// </returns>
        /// <exception cref="ApplicationException">Thrown if the grant type is unsupported.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown if the client credentials are invalid.</exception>
        public async Task<object> Handle(ClientCredentialsCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate grant type
            if (request.Grant_Type != "client_credentials")
            {
                throw new ApplicationException("unsupported_grant_type");
            }

            // 2. Authenticate client
            var clientIdentity = await _clientRepository.FindClientByCredentialsAsync(
                request.Client_Id, request.Client_Secret);

            if (clientIdentity == null)
            {
                throw new UnauthorizedAccessException("Invalid client credentials.");
            }

            // 3. Generate JWT access token
            var accessToken = _jwtProvider.GenerateForService(clientIdentity.ClientId, clientIdentity.Scope);

            // 4. Return standard response
            return new
            {
                access_token = accessToken,
                token_type = "Bearer",
                expires_in = 900 // 15 minutes
            };
        }
    }
}
