using System.Diagnostics.CodeAnalysis;

using BenchmarkDotNet.Attributes;

namespace Audentity.Benchmarks;

[MemoryDiagnoser]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class Executor
{
    private readonly Database _database = new();

    public Executor()
    {
        IEnumerable<Tenant> tenants = Enumerable.Range(1, Count).Select(i =>
        {
            Tenant tenant = new() { Id = Guid.NewGuid(), Name = $"Tenant{i}" };
            tenant.Projects = Enumerable.Range(1, 3)
                .Select(pi => new Project { Id = Guid.NewGuid(), Name = $"Project{pi}", Tenant = tenant })
                .ToArray();

            tenant.Users = Enumerable.Range(1, 3).Select(ui => new User
            {
                Id = Guid.NewGuid(),
                Name = $"User{ui}",
                Address = new Address
                {
                    Street = $"Street{ui}",
                    City = $"City{ui}",
                    State = $"State{ui}",
                    ZipCode = new ZipCode($"Zip{ui}")
                }
            }).ToArray();

            // Assign all users to each project.
            foreach (User user in tenant.Users)
            {
                user.Projects = tenant.Projects;
            }

            // Assign all projects to each user.
            foreach (Project project in tenant.Projects)
            {
                project.Users = tenant.Users;
            }

            return tenant;
        });
        _database.Set<Tenant>().AddRange(tenants);
    }

    [Params(1, 10, 100, 1000)]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public int Count { get; set; }

    [Benchmark]
    public void Collect()
    {
        foreach (Trace _ in _database.ChangeTracker.Entries().Collect())
        {
            // Ignore.
        }
    }
}