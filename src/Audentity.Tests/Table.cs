namespace Audentity.Tests;

public class Table
{
    public Guid Id { get; set; }
    public string Property { get; set; } = String.Empty;
    public OwnedTable OwnedProperty { get; set; } = new(String.Empty);
}

public record OwnedTable(string Value);