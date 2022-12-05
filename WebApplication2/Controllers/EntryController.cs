using DALContracts;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.Controllers;

[ApiController]
[AllowAnonymous]
[Route("[controller]")]
public class EntryController : ControllerBase
{
    private readonly IEntryRepository _entryRepository;

    public EntryController(IEntryRepository entryRepository) =>
        _entryRepository = entryRepository;

    [HttpGet("{id}")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<Entry> GetEntry(Guid id)
    {
        var entry = await _entryRepository.GetByIdAsync(id, CancellationToken.None);

        if (entry == null)
            throw new Exception("Entry was not found");
        
        return entry;
    }
    
    [HttpGet("Entries")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IEnumerable<Entry>> GetEntries()
    {
        return await _entryRepository.GetAllAsync(CancellationToken.None);
    }

    [HttpGet("WithData/{id}")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<Entry> GetEntryWithData(Guid id)
    {
        var entryWithDataIncluded = await _entryRepository.GetWithDataAsync(id, CancellationToken.None);
        
        if (entryWithDataIncluded == null)
            throw new Exception("Entry was not found");
        
        return entryWithDataIncluded;
    }
    
    [HttpGet("EntryText/{id}")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<string> GetEntryText(Guid id)
    {
        var entryText = await _entryRepository.GetEntryTextAsync(id, CancellationToken.None);
        
        if (entryText == null)
            throw new Exception("Entry was not found");
        
        return entryText;
    }
    
    [HttpGet("WithSameTextAndDescriptionExists/{id}")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<bool> GetAnyEntryHasTheSameTextAndDescription(Guid id)
    {
        return await _entryRepository.AnyEntryHasSameTextAndDescriptionAsync(id, CancellationToken.None);
    }

    [HttpPost]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task AddEntry([FromBody] AddEntryCommand command)
    {
        var entry = new Entry
        {
            Text = command.EntryText, 
            EntryDatas = new List<EntryData> { new() {Amount = command.Amount, Description = command.Description} }
        };
        
        await _entryRepository.SaveEntryAsync(entry, CancellationToken.None);
    }

    [HttpDelete("/Entry/{id}")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public Task RemoveEntry(Guid id) => 
        _entryRepository.RemoveEntryAsync(id, CancellationToken.None);

    [HttpPut("/Entry/{id}")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public Task UpdateEntry(Guid id, [FromBody] string text) => 
        _entryRepository.UpdateEntryTextAsync(id, text, CancellationToken.None);
}