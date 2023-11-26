using System.Collections.Immutable;

using Audentity.Tracking;

namespace Audentity;

public static class TraceExtensions
{
    public static IEnumerable<Trace> Link(this IEnumerable<Trace> traces)
    {
        traces = traces.ToImmutableList();

        ImmutableList<Trace> pool = traces.Where(t => t.IsOwned).ToImmutableList();

        return traces.Where(t => !t.IsOwned)
            .Select(t =>
            {
                IEnumerable<Property> linkedProperties = LinkProperties(t, pool).Where(p => !p.IsPrimaryKey);
                return t with { Properties = t.Properties.Concat(linkedProperties).ToImmutableList() };
            });
    }

    private static IEnumerable<Property> LinkProperties(Trace trace, ImmutableList<Trace> pool)
    {
        foreach (Reference reference in trace.References)
        {
            if (pool.SingleOrDefault(t => IsLinked(reference, t)) is not { } linked)
            {
                continue;
            }

            foreach (Property property in linked.Properties)
            {
                yield return ToLinkedProperty(property, reference);
            }

            foreach (Property property in LinkProperties(linked, pool))
            {
                yield return ToLinkedProperty(property, reference);
            }
        }

        yield break;

        static Property ToLinkedProperty(Property property, Reference reference)
        {
            string name = $"{reference.Name}/{property.Name}";
            return property with { Name = name };
        }
    }

    private static bool IsLinked(Reference reference, Trace trace)
    {
        return trace.Name == reference.Target && IsLinked(reference.Links, trace.Properties);
    }

    private static bool IsLinked(IEnumerable<Link> links, IEnumerable<Property> properties)
    {
        ImmutableList<Property> keys = properties.Where(p => p.IsPrimaryKey).ToImmutableList();
        return links.All(l => keys.Any(p => p.Name == l.Name && p.CurrentValue == l.Value));
    }
}