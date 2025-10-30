namespace DynamoDB.Net.Serialization.Converters;

/// <summary>
/// Represents a type converter that can deserialize DynamoDB number <c>N</c> values.
/// </summary>
public interface IConvertFromNumber
{
    /// <summary>
    /// Deserializes the number value into an instance of <paramref name="toType"/>.
    /// </summary>
    /// <param name="value">The number value.</param>
    /// <param name="toType">The target type.</param>
    /// <returns>The deserialized value.</returns>
    object ConvertFromNumber(string value, Type toType);
}
