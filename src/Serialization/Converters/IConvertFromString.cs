namespace DynamoDB.Net.Serialization.Converters;

/// <summary>
/// Represents a type converter that can deserialize DynamoDB string <c>S</c> values.
/// </summary>
public interface IConvertFromString
{
    /// <summary>
    /// Deserializes the string value into an instance of <paramref name="toType"/>.
    /// </summary>
    /// <param name="value">The string value.</param>
    /// <param name="toType">The target type.</param>
    /// <returns>The deserialized value.</returns>
    object ConvertFromString(string value, Type toType);
}
