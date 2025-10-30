namespace DynamoDB.Net.Serialization.Converters;

/// <summary>
/// Base class for type converters used when serializing and deserializing DynamoDB data types.
/// </summary>
public abstract class DynamoDBTypeConverter
{
    /// <summary>
    /// Default fallback type converter instance used when no specialized type converter is specified for the type.
    /// </summary>
    public static DynamoDBTypeConverter Default { get; } = new DefaultDynamoDBTypeConverter();

    /// <summary>
    /// Determines whether this type converter can handle a specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type to deserialize or serialize.</param>
    /// <returns><c>true</c> when this converter can serialize and/or deserialize the given <paramref name="type"/>, otherwise <c>false</c>.</returns>
    public abstract bool Handle(Type type);
}
