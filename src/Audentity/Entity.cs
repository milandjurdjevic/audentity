using Microsoft.EntityFrameworkCore;

namespace Audentity;

public record Entity
{
    public IEnumerable<Property> Properties { get; internal init; } = Enumerable.Empty<Property>();
    public EntityState State { get; internal init; }
    public string Name { get; internal init; } = String.Empty;
}