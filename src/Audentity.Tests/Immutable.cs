namespace Audentity.Tests;

public record Immutable
{
    public string Value { get; init; } = Guid.NewGuid().ToString();
}