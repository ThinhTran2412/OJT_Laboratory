using IAM_Service.Application.DTOs;
using MediatR;

namespace IAM_Service.Application.Users.Query
{
    /// <seealso cref="MediatR.IRequest&lt;System.Collections.Generic.List&lt;IAM_Service.Application.DTOs.UserDTO&gt;&gt;" />
    public class GetUserQuery : IRequest<List<UserDTO>>
    {
        // ====== FILTER ======
        /// <summary>
        /// Gets or sets the keyword.
        /// </summary>
        /// <value>
        /// The keyword.
        /// </value>
        public string? Keyword { get; set; }
        /// <summary>
        /// Gets or sets the filter field.
        /// </summary>
        /// <value>
        /// The filter field.
        /// </value>
        public string? FilterField { get; set; }
        /// <summary>
        /// Gets or sets the gender.
        /// </summary>
        /// <value>
        /// The gender.
        /// </value>
        public string? Gender { get; set; }
        /// <summary>
        /// Gets or sets the minimum age.
        /// </summary>
        /// <value>
        /// The minimum age.
        /// </value>
        public int? MinAge { get; set; }
        /// <summary>
        /// Gets or sets the maximum age.
        /// </summary>
        /// <value>
        /// The maximum age.
        /// </value>
        public int? MaxAge { get; set; }

        // ====== SORT ======
        /// <summary>
        /// Gets or sets the sort by.
        /// </summary>
        /// <value>
        /// The sort by.
        /// </value>
        public string? SortBy { get; set; }
        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        /// <value>
        /// The sort order.
        /// </value>
        public string? SortOrder { get; set; }
    }
}
