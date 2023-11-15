using System.Linq.Expressions;

using FluentAssertions.Collections;

namespace Audentity.Tests;

public static class AssertionExtensions
{
    public static AndWhichConstraint<GenericCollectionAssertions<Property>, Property>
        ContainProperty<TEntity, TProperty>(this GenericCollectionAssertions<Property> assertions,
            Expression<Func<TEntity, TProperty>> selector, TProperty currentValue)
    {
        string? currentValueString = currentValue?.ToString();
        string name = selector.GetMemberName();
        return assertions.Contain(property =>
            property.Name == name &&
            property.OriginalValue == null &&
            property.CurrentValue == currentValueString);
    }

    private static string GetMemberName<TEntity, TProperty>(this Expression<Func<TEntity, TProperty>> selector)
    {
        return String.Join(":", GetNameSegments().Reverse());

        IEnumerable<string> GetNameSegments()
        {
            Expression? expression = selector.Body;
            while (expression is MemberExpression memberExpression)
            {
                expression = memberExpression.Expression;
                yield return memberExpression.Member.Name;
            }
        }
    }

    public static AndWhichConstraint<GenericCollectionAssertions<Property>, Property>
        ContainModifiedProperty<TEntity, TProperty>(this GenericCollectionAssertions<Property> assertions,
            Expression<Func<TEntity, TProperty>> selector, TProperty originalValue, TProperty currentValue)
    {
        string? originalValueString = originalValue?.ToString();
        string? currentValueString = currentValue?.ToString();
        string memberName = selector.GetMemberName();
        return assertions.Contain(property =>
            property.Name == memberName &&
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