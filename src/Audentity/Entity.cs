using Microsoft.EntityFrameworkCore;

namespace Audentity;

public record Entity
{
    public IEnumerable<Property> Properties { get; init; } = Enumerable.Empty<Property>();
    public EntityState State { get; init; }
    public string Name { get; init; } = String.Empty;
}