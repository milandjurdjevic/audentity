namespace Audentity;

public record Property(string Name)
{
    public string? CurrentValue { get; init; }
    public string? OriginalValue { get; init; }
    public bool IsPrimaryKey { get; init; }
    public bool IsForeignKey { get; init; }
}