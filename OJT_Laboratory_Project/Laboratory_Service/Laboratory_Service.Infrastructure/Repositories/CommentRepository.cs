using Laboratory_Service.Application.Interface;
using Laboratory_Service.Domain.Entity;
using Laboratory_Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Laboratory_Service.Application.Interface.ICommentRepository" />
    public class CommentRepository : ICommentRepository
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public CommentRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets the by identifier asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<Comment?> GetByIdAsync(int id)
        {
            return await _context.Comment
                .FirstOrDefaultAsync(x => x.CommentId == id);
        }


        /// <summary>
        /// Adds the asynchronous.
        /// </summary>
        /// <param name="comment">The comment.</param>
        public async Task AddAsync(Comment comment)
        {
            await _context.Comment.AddAsync(comment);
        }

        /// <summary>
        /// Saves the changes asynchronous.
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Comment comment)
        {
            _context.Comment.Update(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Comment>> GetByTestResultIdAsync(List<int> testResultId)
        {
            // Kiểm tra danh sách đầu vào để tránh lỗi truy vấn nếu danh sách rỗng hoặc null
            if (testResultId == null || !testResultId.Any())
            {
                return new List<Comment>();
            }

            return await _context.Comment
                // Sử dụng Contains() để kiểm tra xem c.TestResultId có nằm trong danh sách testResultId hay không
                .Where(c => testResultId.Contains(c.TestResultId.Value) && !c.IsDeleted)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Comment>> GetByTestOrderIdAsync(Guid testOrderId)
        {
            return await _context.Comment
                .Where(c => c.TestOrderId == testOrderId && !c.IsDeleted)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }
    }
}
