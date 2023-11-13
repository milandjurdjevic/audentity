using System.Linq.Expressions;

using FluentAssertions.Collections;

namespace Audentity.Tests;

public static class AssertionExtensions
{
    public static AndWhichConstraint<GenericCollectionAssertions<Property>, Property>
        ContainAddedProperty<TEntity, TProperty>(this GenericCollectionAssertions<Property> assertions,
            Expression<Func<TEntity, TProperty>> selector, TProperty currentValue)
    {
        string? currentValueString = currentValue?.ToString();
        return assertions.Contain(property =>
            property.Name == ((MemberExpression)selector.Body).Member.Name &&
            property.OriginalValue == null &&
            property.CurrentValue == currentValueString);
    }

    public static AndWhichConstraint<GenericCollectionAssertions<Property>, Property>
        ContainModifiedProperty<TEntity, TProperty>(this GenericCollectionAssertions<Property> assertions,
            Expression<Func<TEntity, TProperty>> selector, TProperty originalValue, TProperty currentValue)
    {
        string? originalValueString = originalValue?.ToString();
        string? currentValueString = currentValue?.ToString();
        return assertions.Contain(property =>
            property.Name == ((MemberExpression)selector.Body).Member.Name &&
            property.OriginalValue == originalValueString &&
            property.CurrentValue == currentValueString);
    }

    public static AndConstraint<GenericCollectionAssertions<Property>> NotContain<TEntity, TProperty>(
        this GenericCollectionAssertions<Property> assertions, Expression<Func<TEntity, TProperty>> selector)
    {
        return assertions.NotContain(property => property.Name == ((MemberExpression)selector.Body).Member.Name);
    }

    public static AndConstraint<GenericCollectionAssertions<Property>> ContainPrimaryKey<TEntity, TProperty>(
        this GenericCollectionAssertions<Property> assertions, Expression<Func<TEntity, TProperty>> selector)
    {
        return assertions.Contain(property =>
            property.Name == ((MemberExpression)selector.Body).Member.Name && property.IsPrimaryKey == true);
    }
}