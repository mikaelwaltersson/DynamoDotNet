using System.Reflection;
using Amazon.DynamoDBv2.Model;

namespace DynamoDB.Net.Serialization;

/// <summary>
/// Abstraction for serializing into and deserialization from DynamoDB AttributeValue representations.
/// </summary>
public interface IDynamoDBSerializer
{
    /// <summary>
    /// Deserialize a DynamoDB AttributeValue into an instance of the given <paramref name="objectType"/>.
    /// </summary>
    /// <param name="value">DynamoDB attribute value to deserialize.</param>
    /// <param name="objectType">The target type.</param>
    /// <returns>Deserialized instance of <paramref name="objectType"/> or null.</returns>
    object? DeserializeDynamoDBValue(AttributeValue value, Type objectType);

    /// <summary>
    /// Deserialize a DynamoDB AttributeValue into an instance of the given <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="value">DynamoDB attribute value to deserialize.</param>
    /// <returns>Deserialized instance of <typeparamref name="T"/> or null.</returns>
    T? DeserializeDynamoDBValue<T>(AttributeValue value) => (T?)DeserializeDynamoDBValue(value, typeof(T));

    /// <summary>
    /// Serialize a value into a DynamoDB AttributeValue.
    /// </summary>
    /// <param name="value">The value to serialize.</param>
    /// <param name="objectType">The target type.</param>
    /// <returns>Serialized AttributeValue representing the given value.</returns>
    AttributeValue SerializeDynamoDBValue(object? value, Type objectType);

    /// <summary>
    /// Serialize a value of type <typeparamref name="T"/> into a DynamoDB AttributeValue.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="value">Value to serialize.</param>
    /// <returns>AttributeValue representation of the value.</returns>
    AttributeValue SerializeDynamoDBValue<T>(T? value) => SerializeDynamoDBValue(value, typeof(T));

    /// <summary>
    /// Returns DynamoDB attribute metadata for the specified property (declaring type + name).
    /// </summary>
    /// <param name="property">Tuple identifying the property (declaring type, property name).</param>
    /// <returns>DynamoDBAttributeInfo describing the property's mapping and settings.</returns>
    DynamoDBAttributeInfo GetPropertyAttributeInfo((Type DeclaringType, string Name) property);

    /// <summary>
    /// Returns DynamoDB attribute metadata for the specified MemberInfo.
    /// </summary>
    /// <param name="property">MemberInfo for the property or field.</param>
    /// <returns>DynamoDBAttributeInfo describing the member's mapping and settings.</returns>
    DynamoDBAttributeInfo GetPropertyAttributeInfo(MemberInfo property) => GetPropertyAttributeInfo(property.AsSimplePropertyReference());

    /// <summary>
    /// Type name resolver.
    /// </summary>
    DynamoDBObjectTypeNameResolver ObjectTypeNameResolver { get; }
}
