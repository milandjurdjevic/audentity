using System.ComponentModel.DataAnnotations;

namespace Audentity.Tests.Fixture;

public class Project
{
    public Guid Id { get; set; }
    [MaxLength(Byte.MaxValue)] public string Name { get; set; } = String.Empty;
    public IEnumerable<User> Users { get; set; } = Enumerable.Empty<User>();
    public Tenant? Tenant { get; set; }
}