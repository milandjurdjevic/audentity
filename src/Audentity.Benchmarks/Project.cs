namespace Audentity.Benchmarks;

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public IEnumerable<User> Users { get; set; } = Enumerable.Empty<User>();
    public Tenant? Tenant { get; set; }
}