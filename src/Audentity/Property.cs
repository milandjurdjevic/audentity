namespace Audentity;

public record Property
{
    public bool IsPrimaryKey { get; init; }
    public string? OriginalValue { get; init; }
    public string? CurrentValue { get; init; }
    public string Name { get; init; } = String.Empty;
    public string Owner { get; init; } = String.Empty;
}