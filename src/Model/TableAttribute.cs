namespace DynamoDB.Net.Model;

/// <summary>
/// Attribute used to specify the DynamoDB table name for a type.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class TableAttribute : Attribute
{
    /// <summary>
    /// Optional explicit table name to use for the type.
    /// </summary>
    public string? TableName { get; init; }
}
