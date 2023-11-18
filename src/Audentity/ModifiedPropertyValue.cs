namespace Audentity;

public record ModifiedPropertyValue(string Current, string Original) : PropertyValue(Current);