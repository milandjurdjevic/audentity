namespace Audentity.Tests;

public class Model
{
    public Guid Id { get; set; }
    public string Property { get; set; } = Guid.NewGuid().ToString();
    public Owned? Owned { get; set; } = new();
}