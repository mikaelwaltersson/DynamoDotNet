namespace DynamoDB.Net.Serialization.Converters;

/// <summary>
/// Represents a type converter that can deserialize DynamoDB boolean <c>BOOL</c> values.
/// </summary>
public interface IConvertFromBoolean
{
    /// <summary>
    /// Deserializes the boolean value into an instance of <paramref name="toType"/>.
    /// </summary>
    /// <param name="value">The boolean value.</param>
    /// <param name="toType">The target type.</param>
    /// <returns>The deserialized value.</returns>
    object ConvertFromBoolean(bool value, Type toType);
}
