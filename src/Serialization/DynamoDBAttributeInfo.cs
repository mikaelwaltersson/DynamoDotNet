namespace DynamoDB.Net.Serialization;

/// <summary>
/// Metadata describing how a member maps to a DynamoDB attribute,
/// including name, serialization rules and optional hooks.
/// </summary>
public sealed class DynamoDBAttributeInfo
{
    /// <summary>
    /// The resolved attribute name used in DynamoDB for the member.
    /// </summary>
    public required string AttributeName { get; init; }

    /// <summary>
    /// True when the member is part of the primary key.
    /// </summary>
    public bool IsPrimaryKey { get; set; }

    /// <summary>
    /// Indicates whether default values should be serialized for this member.
    /// </summary>
    public bool SerializeDefaultValues { get; init; }

    /// <summary>
    /// Indicates whether null values should be serialized for this member.
    /// </summary>
    public bool SerializeNullValues { get; init; }

    /// <summary>
    /// When true the member will not be serialized.
    /// </summary>
    public bool NotSerialized { get; init; }

    /// <summary>
    /// Optional handlers invoked after a property is deserialized.
    /// </summary>
    public IEnumerable<IOnDeserializeProperty> OnDeserializeProperty { get; init; } = [];

    /// <summary>
    /// Optional handlers invoked when a property value is being serialized.
    /// </summary>
    public IEnumerable<IOnSerializeProperty> OnSerializeProperty { get; init; } = [];
}
