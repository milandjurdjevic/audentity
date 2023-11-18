namespace Audentity;

public record PropertyLog
{
    public bool IsPrimaryKey { get; internal init; }
    public string Name { get; internal init; } = String.Empty;
    public string Owner { get; internal init; } = String.Empty;

    public PropertyValue Value { get; internal init; } = PropertyValue.Empty;
}