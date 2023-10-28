
using Nexify.Domain.Entities.Posts;

namespace Nexify.Domain.Interfaces
{
    public interface IPostRepository
    {
        Task AddAsync(Post post);
        Task DeleteAsync(Guid id);
        Task UpdateAsync(Post post);
        Task<Post> GetByIdAsync(Guid id);
        Task<IEnumerable<Post>> GetAllAsync();
    }
}
