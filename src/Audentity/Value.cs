namespace Audentity;

public record Value(string? Current, string? Original)
{
    public static readonly Value Default = new(null, null);
}