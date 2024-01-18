using System.ComponentModel.DataAnnotations;

namespace Audentity.Benchmarks;

public class User
{
    public Guid Id { get; set; }

    [MaxLength(Byte.MaxValue)] public string Name { get; set; } = String.Empty;

    public Address? Address { get; set; } = new();
    public IEnumerable<Project> Projects { get; set; } = Enumerable.Empty<Project>();
}