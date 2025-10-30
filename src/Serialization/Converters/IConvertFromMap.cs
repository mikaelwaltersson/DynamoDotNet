using Amazon.DynamoDBv2.Model;

namespace DynamoDB.Net.Serialization.Converters;

/// <summary>
/// Represents a type converter that can deserialize DynamoDB map <c>M</c> values.
/// </summary>
public interface IConvertFromMap
{
    /// <summary>
    /// Deserializes the map entries into an instance of <paramref name="toType"/>.
    /// </summary>
    /// <param name="entries">The map entries.</param>
    /// <param name="toType">The target type.</param>
    /// <param name="serializer">Serializer to use for deserialization for nested values.</param>
    /// <returns>The deserialized value.</returns>
    object ConvertFromMap(Dictionary<string, AttributeValue> entries, Type toType, IDynamoDBSerializer serializer);
}
