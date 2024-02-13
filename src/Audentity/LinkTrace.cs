using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Audentity;

public record LinkTrace(string Name, string Value)
{
    internal static LinkTrace FromProperty(IProperty property, object entity)
    {
        string value = property.PropertyInfo?.GetValue(entity)?.ToString() ?? String.Empty;
        return new LinkTrace(property.Name, value);
    }

    internal static LinkTrace FromEntry(PropertyEntry entry)
    {
        string value = entry.CurrentValue?.ToString() ?? String.Empty;
        return new LinkTrace(entry.Metadata.Name, value);
    }
}