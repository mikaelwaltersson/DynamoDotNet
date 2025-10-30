namespace DynamoDB.Net.Model;

/// <summary>
/// Attribute that controls how a property or field is mapped to a DynamoDB attribute.
/// Use to override the attribute name and control serialization of default/null values.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class DynamoDBPropertyAttribute : Attribute
{
    (bool Value, bool IsSpecified) serializeDefaultValues;
    (bool Value, bool IsSpecified) serializeNullValues;

    /// <summary>
    /// Optional explicit DynamoDB attribute name to use instead of the member name.
    /// </summary>
    public string? AttributeName { get; init; }

    /// <summary>
    /// When true, default values (e.g. 0 for numbers, false for booleans) are serialized.
    /// </summary>
    public bool SerializeDefaultValues
    {
        get => serializeDefaultValues.Value;
        set => serializeDefaultValues = (value, true);
    }

    /// <summary>
    /// Indicates whether <see cref="SerializeDefaultValues"/> has been explicitly set.
    /// </summary>
    public bool SerializeDefaultValuesIsSpecified => serializeDefaultValues.IsSpecified;

    /// <summary>
    /// When true, null values will be serialized for this member.
    /// </summary>
    public bool SerializeNullValues
    {
        get => serializeNullValues.Value;
        set => serializeNullValues = (value, true);
    }

    /// <summary>
    /// Indicates whether <see cref="SerializeNullValues"/> has been explicitly set.
    /// </summary>
    public bool SerializeNullValuesIsSpecified => serializeNullValues.IsSpecified;

    /// <summary>
    /// When true, the member will be ignored by the serializer and not written to DynamoDB.
    /// </summary>
    public bool NotSerialized { get; init; } 
}
