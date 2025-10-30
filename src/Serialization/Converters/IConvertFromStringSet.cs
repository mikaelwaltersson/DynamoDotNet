namespace DynamoDB.Net.Serialization.Converters;


/// <summary>
/// Represents a type converter that can deserialize DynamoDB string set <c>SS</c> values.
/// </summary>
public interface IConvertFromStringSet
{
    /// <summary>
    /// Deserializes the string set value into an instance of <paramref name="toType"/>.
    /// </summary>
    /// <param name="values">The string set value.</param>
    /// <param name="toType">The target type.</param>
    /// <param name="convertFromString">Type converter used for the individual string elements in the set.</param>
    /// <returns>The deserialized value.</returns>
    object ConvertFromStringSet(ICollection<string> values, Type toType, IConvertFromString convertFromString);
}
