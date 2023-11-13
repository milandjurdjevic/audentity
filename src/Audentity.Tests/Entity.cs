namespace Audentity.Tests;

public class Entity
{
    public Guid Id { get; set; }
    public string Property { get; set; } = String.Empty;
    public Owned Owned { get; set; } = new(String.Empty);
}

public record Owned(string Value);