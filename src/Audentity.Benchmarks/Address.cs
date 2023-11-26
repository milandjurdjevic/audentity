namespace Audentity.Benchmarks;

public record Address
{
    public string Street { get; init; } = String.Empty;
    public string City { get; init; } = String.Empty;
    public string State { get; init; } = String.Empty;
    public ZipCode ZipCode { get; init; } = new(String.Empty);
}