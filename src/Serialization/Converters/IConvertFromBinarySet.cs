namespace DynamoDB.Net.Serialization.Converters;

/// <summary>
/// Represents a type converter that can deserialize DynamoDB binary set <c>BS</c> values.
/// </summary>
public interface IConvertFromBinarySet
{
    /// <summary>
    /// Deserializes the binary set value into an instance of <paramref name="toType"/>.
    /// </summary>
    /// <param name="values">The binary set value.</param>
    /// <param name="toType">The target type.</param>
    /// <param name="convertFromBinary">Type converter used for the individual binary elements in the set.</param>
    /// <returns>The deserialized value.</returns>
    object ConvertFromBinarySet(ICollection<MemoryStream> values, Type toType, IConvertFromBinary convertFromBinary);
}
