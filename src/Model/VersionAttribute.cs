namespace DynamoDB.Net.Model;

/// <summary>
/// Marks a property or field as the version used for   
/// optimistic concurrency checks in DynamoDB operations.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class VersionAttribute : Attribute
{
}
