namespace DynamoDB.Net.Serialization;

/// <summary>
/// Delegate to determine if default values should be serialized or not per type.
/// </summary>
public delegate bool SerializeDefaultValuesForDelegate(Type propertyType);
