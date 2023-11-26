namespace Audentity.Tests;

public static class Seeding
{
    public static IEnumerable<Tenant> Seed(int count = 1)
    {
        return Enumerable.Range(1, count + 1).Select(i =>
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
                    Building = new Building
                    {
                        Apartment = ui + 10,
                        Floor = ui,
                        Number = new BuildingNumber { Prefix = ui, Suffix = "A" }
                    },
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
    }
}