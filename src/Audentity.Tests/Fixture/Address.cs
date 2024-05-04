namespace Audentity.Tests.Fixture;

public record Address
{
    public string Street { get; init; } = String.Empty;
    public Building Building { get; init; } = new();
    public string City { get; init; } = String.Empty;
    public string State { get; init; } = String.Empty;
    public ZipCode ZipCode { get; init; } = new(String.Empty);
}

public record Building
{
    public BuildingNumber Number { get; init; } = new();
    public int Floor { get; init; }
    public int Apartment { get; init; }
}

public record BuildingNumber
{
    public int Prefix { get; set; }
    public string Suffix { get; init; } = String.Empty;
}