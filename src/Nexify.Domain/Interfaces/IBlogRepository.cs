﻿using Nexify.Domain.Entities.Pagination;
using Nexify.Domain.Entities.Posts;

namespace Nexify.Domain.Interfaces
{
    public interface IBlogRepository
    {
        public Task AddAsync(Post post);
        public Task DeleteAsync(Guid id);
        public Task ModifyAsync(Post post);
        public Task<PagedResult<Post>> RetrieveAllAsync(PaginationFilter validFilter);
    }
}
