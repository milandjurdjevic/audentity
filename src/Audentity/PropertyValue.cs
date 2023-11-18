namespace Audentity;

public record PropertyValue(string Current)
{
    public static readonly PropertyValue Empty = new(String.Empty);
}