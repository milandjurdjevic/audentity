## Getting started

To collect traces, you want to catch the state of the `Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker`
before saving changes. That can be done simply by overriding the `SaveChanges()` & `SaveChangesAsync(CancellationToken)`
methods in your `Microsoft.EntityFrameworkCore.DbContext` implementation.

```csharp
public class Database : DbContext
{
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        IEnumerable<Entity> entities = Tracker.OfEntities(ChangeTracker.Entries());
        return await base.SaveChangesAsync(cancellationToken);
    }
}
```
### Entity Trace Structure (JSON)

Each `Entity` consist of the following properties:

- Name - name of the entity that is being tracked.
- Properties - list of entity properties and their values.
- References - list of entity references and their links.
- State - current state of the entity.

```json
  {
    "Name": "Audentity.Tests.User",
    "Properties": [
      {
        "CurrentValue": "Guid_5",
        "OriginalValue": "Guid_5",
        "Name": "Id"
      },
      {
        "CurrentValue": "User1",
        "OriginalValue": "User1",
        "Name": "Name"
      },
      {
        "CurrentValue": "Guid_1",
        "OriginalValue": "Guid_1",
        "Name": "TenantId"
      }
    ],
    "References": [
      {
        "Links": [
          {
            "Name": "UserId",
            "Value": "Guid_5"
          }
        ],
        "Name": "Address",
        "Target": "Audentity.Tests.Address"
      },
      {
        "Links": [
          {
            "Name": "Id",
            "Value": "Guid_2"
          }
        ],
        "Name": "Projects",
        "Target": "Audentity.Tests.Project"
      },
      {
        "Links": [
          {
            "Name": "Id",
            "Value": "Guid_3"
          }
        ],
        "Name": "Projects",
        "Target": "Audentity.Tests.Project"
      },
      {
        "Links": [
          {
            "Name": "Id",
            "Value": "Guid_4"
          }
        ],
        "Name": "Projects",
        "Target": "Audentity.Tests.Project"
      }
    ],
    "State": "Added"
  }
```