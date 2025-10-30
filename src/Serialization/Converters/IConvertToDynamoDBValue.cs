using Amazon.DynamoDBv2.Model;

namespace DynamoDB.Net.Serialization.Converters;

/// <summary>
/// Represents a type converter that can serialize values into DynamoDB <c>AttributeValue</c> instances.
/// </summary>
public interface IConvertToDynamoDBValue
{
    /// <summary>
    /// Serializes the provided value into a DynamoDB <c>AttributeValue</c> instance.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="fromType">The declared type of the value.</param>
    /// <param name="serializer">The serializer instance.</param>
    /// <returns>The serialized value.</returns>
    AttributeValue ConvertToDynamoDBValue(object? value, Type fromType, IDynamoDBSerializer serializer);
}
