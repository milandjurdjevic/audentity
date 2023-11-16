namespace Audentity.Tests;

public class AuditableTests
{
    [Theory]
    [AutoData]
    public void Collect_SingleAdded_ChangeHasPrimaryKeyProperty(Database database, RootEntity entity)
    {
        database.Set<RootEntity>().Add(entity);
        database.ChangeTracker.Audit().Single().Properties.Should()
            .ContainPrimaryKey<RootEntity, Guid>(e => e.Id);
    }

    [Theory]
    [AutoData]
    public void Collect_SingleModified_ChangeHasModifiedProperty(Database database, RootEntity entity,
        string currentValue)
    {
        database.Set<RootEntity>().Add(entity);
        database.SaveChanges();
        string originalValue = entity.Property;
        entity.Property = currentValue;

        database.ChangeTracker.Audit().Single().Properties.Should()
            .ContainModifiedProperty<RootEntity, string>(e => e.Property, originalValue, currentValue);
    }

    [Theory]
    [AutoData]
    public void Collect_SingleOwnedModified_ChangeHasModifiedProperty(Database database, RootEntity entity,
        OwnedEntity current)
    {
        database.Set<RootEntity>().Add(entity);
        database.SaveChanges();
        OwnedEntity original = entity.Owned;
        entity.Owned = current;

        database.ChangeTracker.Audit().Single().Properties.Should()
            .ContainModifiedProperty<RootEntity, string>(e => e.Owned.Value, original.Value, current.Value);
    }

    [Theory]
    [AutoData]
    public void Collect_SingleAdded_ChangeHasAllPropertiesAndTheirValues(Database database, RootEntity entity)
    {
        database.Set<RootEntity>().Add(entity);
        IEnumerable<Entity> enumerable = database.ChangeTracker.Audit();
        enumerable.Single().Properties.Should()
            .ContainProperty<RootEntity, Guid>(e => e.Id, entity.Id)
            .And
            .ContainProperty<RootEntity, string>(e => e.Property, entity.Property)
            .And
            .ContainProperty<RootEntity, string>(e => e.Owned.Value, entity.Owned.Value)
            .And
            .ContainProperty<RootEntity, string>(e => e.Owned.Nested.Value, entity.Owned.Nested.Value);
    }

    [Theory]
    [AutoData]
    public void Collect_RangeAdded_ChangesHaveCountOfTrackedEntities(Database database, RootEntity[] entities)
    {
        database.Set<RootEntity>().AddRange(entities);
        database.ChangeTracker.Audit().Should().HaveCount(entities.Length);
    }

    [Theory]
    [AutoData]
    public void Collect_SingleAdded_ChangeHasEntityName(Database database, RootEntity entity)
    {
        database.Set<RootEntity>().Add(entity);
        string? entityName = entity.GetType().FullName;
        database.ChangeTracker.Audit().Single().Name.Should().Be(entityName);
    }

    [Theory]
    [AutoData]
    public void Collect_SingleAdded_ChangeHasAddedState(Database database, RootEntity entity)
    {
        database.Set<RootEntity>().Add(entity);
        database.ChangeTracker.Audit().Single().State.Should().Be(EntityState.Added);
    }

    [Theory]
    [AutoData]
    public void Collect_SingleModified_ChangeHasModifiedState(Database database, RootEntity entity, string name)
    {
        database.Set<RootEntity>().Add(entity);
        database.SaveChanges();
        entity.Property = name;
        database.ChangeTracker.Audit().Single().State.Should().Be(EntityState.Modified);
    }

    [Theory]
    [AutoData]
    public void Collect_SingleDeleted_ChangeHasDeletedState(Database database, RootEntity entity)
    {
        database.Set<RootEntity>().Add(entity);
        database.SaveChanges();
        database.Set<RootEntity>().Remove(entity);
        database.ChangeTracker.Audit().Single().State.Should().Be(EntityState.Deleted);
    }
}