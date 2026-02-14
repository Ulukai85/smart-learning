namespace SmartLearning.DTOs;

public class DeckDto
{
    public required Guid Id {get; set;}
    public required string OwnerUserId {get; set;}
    public required string Name {get; set;}
    public string Description { get; set; } = string.Empty;
}

public class CreateDeckDto
{
    public required string Name {get; set;}
    public string Description {get; set;} = string.Empty;
}
