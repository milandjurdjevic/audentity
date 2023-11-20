namespace Audentity.Tests;

public record Owned
{
    public string Value { get; set; } = Guid.NewGuid().ToString();
    public Immutable? Immutable { get; set; } = new();
}