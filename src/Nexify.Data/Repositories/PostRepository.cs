using Nexify.Domain.Entities.Pagination;
using Nexify.Domain.Entities.Posts;
using Nexify.Domain.Interfaces;

namespace Nexify.Data.Repositories
{
    public class PostRepository : IPostRepository
    {
        public Task AddAsync(Post post)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResult<Post>> RetrieveAllAsync(PaginationFilter validFilter)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Post> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Post post)
        {
            throw new NotImplementedException();
        }

    }
}
