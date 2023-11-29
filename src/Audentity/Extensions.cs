using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Audentity;

public static class Extensions
{
    public static IEnumerable<EntityTrace> ToAudit(this IEnumerable<EntityEntry> entries)
    {
        // For N:N relationships, change tracking will generate a "virtual" entry that represents the reference
        // between them, if such an entity does not exist in the source code itself.Those entries will be
        // represented as a string and object dictionary.
        return entries.Where(e => e.Metadata.ClrType != typeof(Dictionary<string, object>)).Select(EntityTrace.Create);
    }
}