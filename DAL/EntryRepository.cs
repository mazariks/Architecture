using DALContracts;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class EntryRepository: IEntryRepository
{
    private readonly ApplicationDbContext _dbContext;

    public EntryRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Entry?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await _dbContext.Entries.SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<IEnumerable<Entry>> GetAllAsync(CancellationToken cancellationToken) =>
        await _dbContext.Entries.ToListAsync(cancellationToken);

    public async Task<Entry?> GetWithDataAsync(Guid id, CancellationToken cancellationToken) =>
        await _dbContext.Entries
            .Include(e => e.EntryDatas)
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<Entry?> GetWithDataAmountLessThanAsync(Guid id, int lessThanAmount, CancellationToken cancellationToken) =>
        await _dbContext.Entries
            .Include(e => e.EntryDatas!.Where(data => data.Amount < lessThanAmount))
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<string?> GetEntryTextAsync(Guid id, CancellationToken cancellationToken) => 
        await _dbContext.Entries
            .Where(e => e.Id == id).Select(e => e.Text)
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<bool> AnyEntryHasSameTextAndDescriptionAsync(Guid id, CancellationToken cancellationToken) => 
        await _dbContext.Entries
            .Include(e => e.EntryDatas)
            .AnyAsync(e => 
                e.EntryDatas!.Any(data => data.Description == e.Text), cancellationToken);

    public async Task SaveEntryAsync(Entry entry, CancellationToken cancellationToken)
    {
        await _dbContext.Entries.AddAsync(entry, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveEntryAsync(Guid id, CancellationToken cancellationToken)
    {
        var existingEntry = await GetByIdAsync(id, cancellationToken);
        if (existingEntry == null)
            throw new Exception("Entry not found");
        
        _dbContext.Entries.Remove(existingEntry);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateEntryTextAsync(Guid id, string text, CancellationToken cancellationToken)
    {
        var existingEntry = await GetByIdAsync(id, cancellationToken);
        if (existingEntry == null)
            throw new Exception("Entry not found");

        existingEntry.Text = text;
        
        await _dbContext.SaveChangesAsync(cancellationToken);

    }
}