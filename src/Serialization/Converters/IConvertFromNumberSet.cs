namespace DynamoDB.Net.Serialization.Converters;

/// <summary>
/// Represents a type converter that can deserialize DynamoDB number set <c>NS</c> values.
/// </summary>
public interface IConvertFromNumberSet
{
    /// <summary>
    /// Deserializes the number set value into an instance of <paramref name="toType"/>.
    /// </summary>
    /// <param name="values">The number set value.</param>
    /// <param name="toType">The target type.</param>
    /// <param name="convertFromNumber">Type converter used for the individual number elements in the set.</param>
    /// <returns>The deserialized value.</returns>
    object ConvertFromNumberSet(ICollection<string> values, Type toType, IConvertFromNumber convertFromNumber);
}
