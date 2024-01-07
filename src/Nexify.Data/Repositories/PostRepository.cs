using Microsoft.EntityFrameworkCore;
using Nexify.Data.Context;
using Nexify.Domain.Entities.Pagination;
using Nexify.Domain.Entities.Posts;
using Nexify.Domain.Interfaces;

namespace Nexify.Data.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly AppDbContext _context;

        public PostRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(Post post)
        {
            _context.Post.Add(post);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResult<Post>> RetrieveAllAsync(PaginationFilter validFilter)
        {
            var pagedData = await _context.Post
                .Include(c => c.Categories)
                .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                .Take(validFilter.PageSize)
                .ToListAsync();

            var totalCount = await _context.Post.CountAsync();

            return new PagedResult<Post> { Items = pagedData, TotalCount = totalCount };
        }

        public async Task DeleteAsync(Guid id)
        {
            var post = await _context.Post.
                Where(x => x.Id == id).FirstOrDefaultAsync();

            post.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        public async Task<Post> GetByIdAsync(Guid id)
        {
            return await _context.Post.
               Include(c => c.Categories).
               Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task ModifyAsync(Post post)
        {
            var currentPost = await _context.Post
                .FirstOrDefaultAsync(p => p.Id == post.Id);

            currentPost.Title = post.Title;
            currentPost.Content = post.Content;
            currentPost.ImageName = post.ImageName;
            currentPost.DateUpdated = DateTime.UtcNow;

            _context.Entry(currentPost).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

    }
}
