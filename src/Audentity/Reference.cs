namespace Audentity;

public record Reference(string Name, Type Type, IReadOnlyCollection<Link> Links);