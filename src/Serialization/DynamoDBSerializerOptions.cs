using DynamoDB.Net.Serialization.Converters;

namespace DynamoDB.Net.Serialization;

/// <summary>
/// Provides programmatic configuration for the <see cref="DynamoDBSerializer" /> instance.
/// </summary>
public class DynamoDBSerializerOptions
{
    /// <summary>
    /// Name transform for property names to DynamoDB attribute names.
    /// </summary>
    public NameTransform AttributeNameTransform { get; set; } = NameTransform.Default;

    /// <summary>
    /// Name transform for enum values to serialized DynamoDB enum values.
    /// </summary>
    public NameTransform EnumValueNameTransform { set => TypeConverters.OfType<EnumTypeConverter>().SetNameTransform(value); }

    /// <summary>
    /// List of type converters to use when serializing and deserializing, 
    /// takes precedence over any built in default serialization logic. 
    /// </summary>
    public List<DynamoDBTypeConverter> TypeConverters { get; set; } =
        [
            new ParsableTypeConverter<DateOnly> { Format = "o" },
            new ParsableTypeConverter<DateTime> { Format = "o" },
            new ParsableTypeConverter<DateTimeOffset> { Format = "o" },
            new ParsableTypeConverter<Guid> { Format = "d" },
            new ParsableTypeConverter<TimeOnly> { Format = "o" },
            new ParsableTypeConverter<TimeSpan> { Format = "c" },
            new EnumTypeConverter(),
            new JsonNodeTypeConverter()
        ];

    /// <summary>
    /// Determines if default values should be serialized or not.
    /// </summary>
    public bool SerializeDefaultValues { get; set; }

    /// <summary>
    /// Optional delegate to determine if default values should be serialized or not per type.
    /// </summary>
    public SerializeDefaultValuesForDelegate? SerializeDefaultValuesFor { get; set; }

    /// <summary>
    /// Determines if null values should be serialized or not.
    /// </summary>
    public bool SerializeNullValues { get; set; }

    /// <summary>
    /// Specify the object type resolver used for polymorphism.
    /// </summary>
    public DynamoDBObjectTypeNameResolver ObjectTypeNameResolver { get; set; } = DynamoDBObjectTypeNameResolver.Default;

    /// <summary>
    /// Optional handlers to be called when properties are being deserialized.
    /// </summary>
    public List<IOnDeserializeProperty> OnDeserializeProperty { get; set; } = [];

    /// <summary>
    /// Optional handlers to be called when properties are being serialized.
    /// </summary>
    public List<IOnSerializeProperty> OnSerializeProperty { get; set; } = [];
}
