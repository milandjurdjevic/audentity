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
    public void ToTraces()
    {
        _ = _database.ChangeTracker
            .Entries()
            .ToTraces();
    }

    [Benchmark]
    public void ToTracesWithAggregateOwned()
    {
        _ = _database.ChangeTracker
            .Entries()
            .ToTraces()
            .AggregateOwned();
    }
}
//
//     | Method                     | Count | Mean      | Error    | StdDev   | Gen0   | Allocated |
//     |--------------------------- |------ |----------:|---------:|---------:|-------:|----------:|
//     | ToTraces                   | 1     |  46.70 ns | 0.144 ns | 0.128 ns | 0.0051 |      32 B |
//     | ToTracesWithAggregateOwned | 1     | 155.01 ns | 0.143 ns | 0.127 ns | 0.0560 |     352 B |
//     | ToTraces                   | 10    |  46.68 ns | 0.053 ns | 0.047 ns | 0.0051 |      32 B |
//     | ToTracesWithAggregateOwned | 10    | 153.37 ns | 0.149 ns | 0.139 ns | 0.0560 |     352 B |
//     | ToTraces                   | 100   |  46.14 ns | 0.047 ns | 0.044 ns | 0.0051 |      32 B |
//     | ToTracesWithAggregateOwned | 100   | 153.82 ns | 0.173 ns | 0.162 ns | 0.0560 |     352 B |
//     | ToTraces                   | 1000  |  46.59 ns | 0.045 ns | 0.042 ns | 0.0051 |      32 B |
//     | ToTracesWithAggregateOwned | 1000  | 154.38 ns | 0.204 ns | 0.191 ns | 0.0560 |     352 B |

//     | Method                     | Count | Mean      | Error    | StdDev   | Gen0   | Allocated |
//     |--------------------------- |------ |----------:|---------:|---------:|-------:|----------:|
//     | ToTraces                   | 1     |  64.98 ns | 0.063 ns | 0.056 ns | 0.0088 |      56 B |
//     | ToTracesWithAggregateOwned | 1     | 272.00 ns | 0.918 ns | 0.814 ns | 0.0610 |     384 B |
//     | ToTraces                   | 10    |  65.23 ns | 0.048 ns | 0.042 ns | 0.0088 |      56 B |
//     | ToTracesWithAggregateOwned | 10    | 213.61 ns | 0.479 ns | 0.448 ns | 0.0610 |     384 B |
//     | ToTraces                   | 100   |  65.83 ns | 0.057 ns | 0.053 ns | 0.0088 |      56 B |
//     | ToTracesWithAggregateOwned | 100   | 212.52 ns | 0.586 ns | 0.548 ns | 0.0610 |     384 B |
//     | ToTraces                   | 1000  |  65.35 ns | 0.054 ns | 0.048 ns | 0.0088 |      56 B |
//     | ToTracesWithAggregateOwned | 1000  | 212.26 ns | 0.584 ns | 0.546 ns | 0.0610 |     384 B |