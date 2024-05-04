using Microsoft.EntityFrameworkCore;

namespace Audentity;

public record Trace(
    Type Type,
    EntityState State,
    IReadOnlyCollection<Property> Properties,
    IReadOnlyCollection<Reference> References,
    bool IsOwned);