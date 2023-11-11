namespace Audentity.Tests;

public class AuditableTests
{
    [Theory]
    [AutoData]
    public void Collect_SingleAdded_ChangeHasPrimaryKeyProperty(Database database, Entity entity)
    {
        database.Set<Entity>().Add(entity);
        Auditable.Collect(database.ChangeTracker).Single().Properties.Should()
            .ContainPrimaryKey<Entity, Guid>(e => e.Id);
    }

    [Theory]
    [AutoData]
    public void Collect_SingleModified_ChangeHasModifiedProperty(Database database, Entity entity, string currentValue)
    {
        database.Set<Entity>().Add(entity);
        database.SaveChanges();
        string originalValue = entity.Property;
        entity.Property = currentValue;

        Auditable.Collect(database.ChangeTracker).Single().Properties.Should()
            .ContainModified<Entity, string>(e => e.Property, originalValue, currentValue);
    }

    [Theory]
    [AutoData]
    public void Collect_SingleAdded_ChangeHasAllPropertiesAndTheirValues(Database database, Entity entity)
    {
        database.Set<Entity>().Add(entity);
        Auditable.Collect(database.ChangeTracker).Single().Properties.Should()
            .ContainUnmodified<Entity, Guid>(e => e.Id, entity.Id, entity.Id)
            .And
            .ContainUnmodified<Entity, string>(e => e.Property, entity.Property, entity.Property);
    }

    [Theory]
    [AutoData]
    public void Collect_RangeAdded_ChangesHaveCountOfTrackedEntities(Database database, Entity[] entities)
    {
        database.Set<Entity>().AddRange(entities);
        Auditable.Collect(database.ChangeTracker).Should().HaveCount(entities.Length);
    }

    [Theory]
    [AutoData]
    public void Collect_SingleAdded_ChangeHasEntityName(Database database, Entity entity)
    {
        database.Set<Entity>().Add(entity);
        string? entityName = entity.GetType().FullName;
        Auditable.Collect(database.ChangeTracker).Single().Name.Should().Be(entityName);
    }

    [Theory]
    [AutoData]
    public void Collect_SingleAdded_ChangeHasAddedState(Database database, Entity entity)
    {
        database.Set<Entity>().Add(entity);
        Auditable.Collect(database.ChangeTracker).Single().State.Should().Be(EntityState.Added);
    }

    [Theory]
    [AutoData]
    public void Collect_SingleModified_ChangeHasModifiedState(Database database, Entity entity, string name)
    {
        database.Set<Entity>().Add(entity);
        database.SaveChanges();
        entity.Property = name;
        Auditable.Collect(database.ChangeTracker).Single().State.Should().Be(EntityState.Modified);
    }

    [Theory]
    [AutoData]
    public void Collect_SingleDeleted_ChangeHasDeletedState(Database database, Entity entity)
    {
        database.Set<Entity>().Add(entity);
        database.SaveChanges();
        database.Set<Entity>().Remove(entity);
        Auditable.Collect(database.ChangeTracker).Single().State.Should().Be(EntityState.Deleted);
    }
}