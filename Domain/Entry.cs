namespace Domain;

public class Entry
{
    public Guid Id { get; set; }
    public string Text { get; set; } = default!;
    
    public ICollection<EntryData>? EntryDatas { get; set; }
}