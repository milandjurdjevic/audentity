namespace Audentity.Benchmarks;

public class Tenant
{
    public Guid Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public IEnumerable<User> Users { get; set; } = Enumerable.Empty<User>();
    public IEnumerable<Project> Projects { get; set; } = Enumerable.Empty<Project>();
}