namespace DynamoDB.Net.Serialization.Converters;

/// <summary>
/// Represents a type converter that can deserialize DynamoDB null <c>NULL</c> values.
/// </summary>
public interface IConvertFromNull
{
    /// <summary>
    /// Deserializes the null value into an instance of <paramref name="toType"/>.
    /// </summary>
    /// <param name="toType">The target type.</param>
    /// <returns>The deserialized value.</returns>
    object? ConvertFromNull(Type toType);
}
