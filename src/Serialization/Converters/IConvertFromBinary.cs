namespace DynamoDB.Net.Serialization.Converters;

/// <summary>
/// Represents a type converter that can deserialize DynamoDB binary <c>B</c> values.
/// </summary>
public interface IConvertFromBinary
{
    /// <summary>
    /// Deserializes the binary value into an instance of <paramref name="toType"/>.
    /// </summary>
    /// <param name="value">The binary value.</param>
    /// <param name="toType">The target type.</param>
    /// <returns>The deserialized value.</returns>
    object ConvertFromBinary(MemoryStream value, Type toType);
}
