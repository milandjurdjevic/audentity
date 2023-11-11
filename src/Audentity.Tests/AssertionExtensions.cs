using System.Linq.Expressions;

using FluentAssertions.Collections;

namespace Audentity.Tests;

public static class AssertionExtensions
{
    public static AndWhichConstraint<GenericCollectionAssertions<Property>, Property>
        ContainUnmodified<TEntity, TProperty>(
            this GenericCollectionAssertions<Property> assertions,
            Expression<Func<TEntity, TProperty>> selector, TProperty originalValue, TProperty currentValue)
    {
        string? originalValueString = originalValue?.ToString();
        string? currentValueString = currentValue?.ToString();
        return assertions.Contain(property =>
            property.Name == ((MemberExpression)selector.Body).Member.Name &&
            property.OriginalValue == originalValueString &&
            property.CurrentValue == currentValueString &&
            property.IsModified == false);
    }

    public static AndWhichConstraint<GenericCollectionAssertions<Property>, Property>
        ContainModified<TEntity, TProperty>(
            this GenericCollectionAssertions<Property> assertions,
            Expression<Func<TEntity, TProperty>> selector, TProperty originalValue, TProperty currentValue)
    {
        return Contain(assertions, selector, originalValue, currentValue, true);
    }

    private static AndWhichConstraint<GenericCollectionAssertions<Property>, Property>
        Contain<TEntity, TProperty>(GenericCollectionAssertions<Property> assertions,
            Expression<Func<TEntity, TProperty>> selector, TProperty originalValue, TProperty currentValue,
            bool modified)
    {
        string? originalValueString = originalValue?.ToString();
        string? currentValueString = currentValue?.ToString();
        return assertions.Contain(property =>
            property.Name == ((MemberExpression)selector.Body).Member.Name &&
            property.OriginalValue == originalValueString &&
            property.CurrentValue == currentValueString &&
            property.IsModified == modified);
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