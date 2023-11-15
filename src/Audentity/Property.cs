namespace Audentity;

public record Property
{
    public bool IsPrimaryKey { get; internal init; }
    public string? OriginalValue { get; internal init; }
    public string? CurrentValue { get; internal init; }
    public string Name { get; internal init; } = String.Empty;
    public string Owner { get; internal init; } = String.Empty;
}