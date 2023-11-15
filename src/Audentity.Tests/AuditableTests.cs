namespace Audentity.Tests;

public class AuditableTests
{
    [Theory]
    [AutoData]
    public void Collect_SingleAdded_ChangeHasPrimaryKeyProperty(Database database, Table entity)
    {
        database.Set<Table>().Add(entity);
        database.ChangeTracker.Audit().Single().Properties.Should()
            .ContainPrimaryKey<Table, Guid>(e => e.Id);
    }

    [Theory]
    [AutoData]
    public void Collect_SingleModified_ChangeHasModifiedProperty(Database database, Table entity, string currentValue)
    {
        database.Set<Table>().Add(entity);
        database.SaveChanges();
        string originalValue = entity.Property;
        entity.Property = currentValue;

        database.ChangeTracker.Audit().Single().Properties.Should()
            .ContainModifiedProperty<Table, string>(e => e.Property, originalValue, currentValue);
    }

    [Theory]
    [AutoData]
    public void Collect_SingleOwnedModified_ChangeHasModifiedProperty(Database database, Table entity,
        OwnedTable current)
    {
        database.Set<Table>().Add(entity);
        database.SaveChanges();
        OwnedTable original = entity.OwnedProperty;
        entity.OwnedProperty = current;

        database.ChangeTracker.Audit().Single().Properties.Should()
            .ContainModifiedProperty<Table, string>(e => e.OwnedProperty.Value, original.Value, current.Value);
    }

    [Theory]
    [AutoData]
    public void Collect_SingleAdded_ChangeHasAllPropertiesAndTheirValues(Database database, Table entity)
    {
        database.Set<Table>().Add(entity);
        IEnumerable<Entity> enumerable = database.ChangeTracker.Audit();
        enumerable.Single().Properties.Should()
            .ContainProperty<Table, Guid>(e => e.Id, entity.Id)
            .And
            .ContainProperty<Table, string>(e => e.Property, entity.Property)
            .And
            .ContainProperty<Table, string>(e => e.OwnedProperty.Value, entity.OwnedProperty.Value);
    }

    [Theory]
    [AutoData]
    public void Collect_RangeAdded_ChangesHaveCountOfTrackedEntities(Database database, Table[] entities)
    {
        database.Set<Table>().AddRange(entities);
        database.ChangeTracker.Audit().Should().HaveCount(entities.Length);
    }

    [Theory]
    [AutoData]
    public void Collect_SingleAdded_ChangeHasEntityName(Database database, Table entity)
    {
        database.Set<Table>().Add(entity);
        string? entityName = entity.GetType().FullName;
        database.ChangeTracker.Audit().Single().Name.Should().Be(entityName);
    }

    [Theory]
    [AutoData]
    public void Collect_SingleAdded_ChangeHasAddedState(Database database, Table entity)
    {
        database.Set<Table>().Add(entity);
        database.ChangeTracker.Audit().Single().State.Should().Be(EntityState.Added);
    }

    [Theory]
    [AutoData]
    public void Collect_SingleModified_ChangeHasModifiedState(Database database, Table entity, string name)
    {
        database.Set<Table>().Add(entity);
        database.SaveChanges();
        entity.Property = name;
        database.ChangeTracker.Audit().Single().State.Should().Be(EntityState.Modified);
    }

    [Theory]
    [AutoData]
    public void Collect_SingleDeleted_ChangeHasDeletedState(Database database, Table entity)
    {
        database.Set<Table>().Add(entity);
        database.SaveChanges();
        database.Set<Table>().Remove(entity);
        database.ChangeTracker.Audit().Single().State.Should().Be(EntityState.Deleted);
    }
}