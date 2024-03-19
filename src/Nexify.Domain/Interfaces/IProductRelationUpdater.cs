namespace Nexify.Domain.Interfaces
{
    public interface IProductRelationUpdater
    {
        Task DeleteRangeAsync(Guid productId);
        Task AddRelationAsync(Guid relationId, Guid productId);
    }
}
