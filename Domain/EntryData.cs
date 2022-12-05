namespace Domain;

public class EntryData
{
    public Guid Id { get; set; }

    public Guid EntryId { get; set; }
    public Entry? Entry;
    
    public int Amount { get; set; }
    public string? Description { get; set; }
}