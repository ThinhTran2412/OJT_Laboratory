using AutoMapper;
using IAM_Service.Application.DTOs;
using IAM_Service.Application.Interface.IUser;
using MediatR;

namespace IAM_Service.Application.Users.Query
{
    /// <summary>
    /// Handle get all user information
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;IAM_Service.Application.Users.Query.GetUserQuery, System.Collections.Generic.List&lt;IAM_Service.Application.DTOs.UserDTO&gt;&gt;" />
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, List<UserDTO>>
    {
        /// <summary>
        /// The users repository
        /// </summary>
        private readonly IUsersRepository _usersRepository;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetUserQueryHandler" /> class.
        /// </summary>
        /// <param name="usersRepository">The users repository.</param>
        /// <param name="mapper">The mapper.</param>
        public GetUserQueryHandler(IUsersRepository usersRepository, IMapper mapper)
        {
            _usersRepository = usersRepository;
            _mapper = mapper;
        }
        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<List<UserDTO>> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var users = await _usersRepository.GetUserAsync();
            if (users == null || !users.Any())
                return new List<UserDTO>();

            // ================= FILTER =================
            if (!string.IsNullOrWhiteSpace(request.FilterField) &&
                !string.IsNullOrWhiteSpace(request.Keyword))
            {
                string keyword = request.Keyword.ToLower();

                users = request.FilterField.ToLower() switch
                {
                    "fullname" => users.Where(u => u.FullName != null && u.FullName.ToLower().Contains(keyword)).ToList(),
                    "email" => users.Where(u => u.Email != null && u.Email.ToLower().Contains(keyword)).ToList(),
                    "address" => users.Where(u => u.Address != null && u.Address.ToLower().Contains(keyword)).ToList(),
                    _ => users
                };
            }
            else if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                string keyword = request.Keyword.ToLower();
                users = users.Where(u =>
                    (u.FullName != null && u.FullName.ToLower().Contains(keyword)) ||
                    (u.Email != null && u.Email.ToLower().Contains(keyword)) ||
                    (u.Address != null && u.Address.ToLower().Contains(keyword))
                ).ToList();
            }

            if (!string.IsNullOrWhiteSpace(request.Gender))
            {
                string gender = request.Gender.ToLower();
                users = users.Where(u => u.Gender != null && u.Gender.ToLower() == gender).ToList();
            }

            if (request.MinAge.HasValue)
                users = users.Where(u => u.Age > request.MinAge.Value).ToList();

            if (request.MaxAge.HasValue)
                users = users.Where(u => u.Age <= request.MaxAge.Value).ToList();


            // ================= SORT =================
            if (!string.IsNullOrWhiteSpace(request.SortBy) || !string.IsNullOrWhiteSpace(request.SortOrder))
            {
                string order = request.SortOrder?.Trim().ToLower();
                bool isAsc = order switch
                {
                    "asc" or "ascending" or "up" => true,
                    "desc" or "descending" or "down" => false,
                    _ => true
                };

                users = request.SortBy?.ToLower() switch
                {
                    "email" => isAsc ? users.OrderBy(u => u.Email).ToList() : users.OrderByDescending(u => u.Email).ToList(),
                    "age" => isAsc ? users.OrderBy(u => u.Age).ToList() : users.OrderByDescending(u => u.Age).ToList(),
                    _ => isAsc ? users.OrderBy(u => u.FullName).ToList() : users.OrderByDescending(u => u.FullName).ToList(),
                };
            }

            // ================= MAP DTO =================
            return _mapper.Map<List<UserDTO>>(users);

        }
    }
}
