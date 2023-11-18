using Microsoft.EntityFrameworkCore;

namespace Audentity;

public record EntityLog
{
    public IEnumerable<PropertyLog> Properties { get; internal init; } = Enumerable.Empty<PropertyLog>();
    public EntityState State { get; internal init; }
    public string Name { get; internal init; } = String.Empty;
}