
using System.Globalization;
using Amazon.DynamoDBv2.Model;

namespace DynamoDB.Net.Serialization.Converters;

/// <summary>
/// Base class for type converters for types that can be parsed from and formatted into string representations. 
/// </summary>
public abstract class ParsableTypeConverter : DynamoDBTypeConverter, IConvertFromString, IConvertToDynamoDBValue
{
    /// <inheritdoc />
    public abstract object ConvertFromString(string value, Type toType);
    
    /// <inheritdoc />
    public abstract AttributeValue ConvertToDynamoDBValue(object? value, Type fromType, IDynamoDBSerializer serializer);
}

/// <summary>
/// Type converter that can parse or format an <typeparamref name="T" /> from/to a string representations. 
/// </summary>
public class ParsableTypeConverter<T> : ParsableTypeConverter
    where T : IParsable<T>, IFormattable
{
    /// <summary>
    /// The format string provided to <see cref="IFormattable.ToString(string?, IFormatProvider?)" />.
    /// </summary>
    public string Format { get; init; } = string.Empty;

    /// <inheritdoc />
    public override bool Handle(Type type) => type.UnwrapNullableType() == typeof(T);

    /// <inheritdoc />
    public override object ConvertFromString(string value, Type toType) =>
        T.Parse(value, CultureInfo.InvariantCulture);

    /// <inheritdoc />
    public override AttributeValue ConvertToDynamoDBValue(object? value, Type fromType, IDynamoDBSerializer serializer)
    {
        if (value == null)
            return new() { NULL = true };

        return new() { S = ((T)value).ToString(Format, CultureInfo.InvariantCulture) };
    }
}
