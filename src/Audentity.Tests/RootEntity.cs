namespace Audentity.Tests;

public class RootEntity
{
    public Guid Id { get; set; }
    public string Property { get; set; } = String.Empty;
    public OwnedEntity Owned { get; set; } = new();
}

public record OwnedEntity
{
    public string Value { get; init; } = String.Empty;
    public NestedOwnedEntity Nested { get; init; } = new();
}

public record NestedOwnedEntity
{
    public string Value { get; set; } = String.Empty;
}