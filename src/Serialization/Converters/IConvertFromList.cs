using Amazon.DynamoDBv2.Model;

namespace DynamoDB.Net.Serialization.Converters;

/// <summary>
/// Represents a type converter that can deserialize DynamoDB list <c>L</c> values.
/// </summary>
public interface IConvertFromList
{
    /// <summary>
    /// Deserializes the list entries into an instance of <paramref name="toType"/>.
    /// </summary>
    /// <param name="elements">The list entries.</param>
    /// <param name="toType">The target type.</param>
    /// <param name="serializer">Serializer to use for deserialization for nested values.</param>
    /// <returns>The deserialized value.</returns>
    object ConvertFromList(List<AttributeValue> elements, Type toType, IDynamoDBSerializer serializer);
}
