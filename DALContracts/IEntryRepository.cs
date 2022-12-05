using Domain;

namespace DALContracts;

public interface IEntryRepository
{
    public Task<Entry?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    public Task<IEnumerable<Entry>> GetAllAsync(CancellationToken cancellationToken);
    public Task<Entry?> GetWithDataAsync(Guid id, CancellationToken cancellationToken);

    public Task<Entry?> GetWithDataAmountLessThanAsync(Guid id, int lessThanAmount, CancellationToken cancellationToken);

    public Task<string?> GetEntryTextAsync(Guid id, CancellationToken cancellationToken);

    public Task<bool> AnyEntryHasSameTextAndDescriptionAsync(Guid id, CancellationToken cancellationToken);

    public Task SaveEntryAsync(Entry entry, CancellationToken cancellationToken);
    public Task RemoveEntryAsync(Guid id, CancellationToken cancellationToken);
    public Task UpdateEntryTextAsync(Guid id, string text, CancellationToken cancellationToken);
}