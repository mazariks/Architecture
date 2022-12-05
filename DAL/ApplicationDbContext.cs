using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class ApplicationDbContext: DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options) {}
    
    public DbSet<Entry> Entries { get; set; } = default!;
    public DbSet<EntryData> EntriesData { get; set; } = default!;
}