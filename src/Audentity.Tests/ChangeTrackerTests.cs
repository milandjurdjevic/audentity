namespace Audentity.Tests;

[UsesVerify]
public class ChangeTrackerTests
{
    private readonly Database _database = new();
    private readonly Tenant _tenant = new() { Id = Guid.NewGuid(), Name = "Tenant1" };

    public ChangeTrackerTests()
    {
        _tenant.Projects = Enumerable.Range(1, 3)
            .Select(i => new Project { Id = Guid.NewGuid(), Name = $"Project{i}", Tenant = _tenant })
            .ToArray();

        _tenant.Users = Enumerable.Range(1, 3).Select(i => new User
        {
            Id = Guid.NewGuid(),
            Name = $"User{i}",
            Address = new Address
            {
                Street = $"Street{i}", City = $"City{i}", State = $"State{i}", ZipCode = new ZipCode($"Zip{i}")
            }
        }).ToArray();

        // Assign all users to each project.
        foreach (User user in _tenant.Users)
        {
            user.Projects = _tenant.Projects;
        }

        // Assign all projects to each user.
        foreach (Project project in _tenant.Projects)
        {
            project.Users = _tenant.Users;
        }
    }

    [Fact]
    public Task Traces_Add_Collects()
    {
        _database.Add(_tenant);
        IEnumerable<Trace> traces = _database.ChangeTracker.Entries().ToTraces();
        return Verify(traces);
    }

    [Fact]
    public Task Traces_Modify_Collects()
    {
        _database.Add(_tenant);
        _database.SaveChanges();
        _tenant.Name += "Updated";
        _tenant.Users.First().Name += "Updated";
        _tenant.Projects.First().Name += "Updated";
        IEnumerable<Trace> traces = _database.ChangeTracker.Entries().ToTraces();
        return Verify(traces);
    }

    [Fact]
    public Task Traces_Delete_Collects()
    {
        _database.Add(_tenant);
        _database.SaveChanges();
        _database.Remove(_tenant);
        IEnumerable<Trace> traces = _database.ChangeTracker.Entries().ToTraces();
        return Verify(traces);
    }
}