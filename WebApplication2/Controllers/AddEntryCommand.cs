namespace WebApplication2.Controllers;

public record AddEntryCommand(string EntryText, int Amount, string Description);