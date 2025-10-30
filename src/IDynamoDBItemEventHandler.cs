using Amazon.DynamoDBv2.Model;
using DynamoDB.Net.Expressions;

namespace DynamoDB.Net;

/// <summary>
/// Handles item lifecycle events during serialization, deserialization and expression translation.
/// Implementations can modify items or translated expressions, for example to handle optimistic concurrency as in <see cref="VersionChecker" />.
/// </summary>
public interface IDynamoDBItemEventHandler
{
    /// <summary>
    /// Invoked after a DynamoDB item has been deserialized, implementations may transform or validate the item.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="item">The deserialized item.</param>
    /// <returns>The item after any modifications by this event handler has been applied.</returns>
    T OnItemDeserialized<T>(T item) where T : class;

    /// <summary>
    /// Invoked after a DynamoDB item has been serialized, implementations may add, remove or modify attributes.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="item">The attribute map representing the serialized item.</param>
    /// <param name="translationContext">The context used for deserialization, serialization and expression translation.</param>
    /// <returns>The attribute map after any modifications by this event handler has been applied.</returns>
    Dictionary<string, AttributeValue> OnItemSerialized<T>(Dictionary<string, AttributeValue> item, ExpressionTranslationContext translationContext) where T : class;

    /// <summary>
    /// Invoked after an update expression has been translated for an item, implementations may modify the translated update expression.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="expression">The translated update expression.</param>
    /// <param name="version">The optional version passed to the update operation.</param>
    /// <param name="translationContext">The context used for deserialization, serialization and expression translation.</param>
    /// <returns>The update expression after any modifications by this event handler has been applied.</returns>
    string OnItemUpdateTranslated<T>(string expression, object? version, ExpressionTranslationContext translationContext) where T : class;

    /// <summary>
    /// Invoked after a condition expression has been translated for an item, implementations may modify the translated condition expression.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="expression">The translated condition expression or <c>null</c> if no condition was specified.</param>
    /// <param name="version">The optional version passed to the put, update, delete or condition check operation.</param>
    /// <param name="translationContext">The context used for deserialization, serialization and expression translation.</param>
    /// <returns>The condition expression after any modifications by this event handler has been applied.</returns>
    string? OnItemConditionTranslated<T>(string? expression, object? version, ExpressionTranslationContext translationContext) where T : class;
}
