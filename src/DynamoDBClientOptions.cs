namespace DynamoDB.Net;

/// <summary>
/// Provides programmatic configuration for the <see cref="DynamoDBClient" /> instance.
/// </summary>
public class DynamoDBClientOptions
{
    /// <summary>
    /// Prefix added to each table name.
    /// </summary>
    public string TableNamePrefix { get; set; } = string.Empty;

    /// <summary>
    /// Table name overrides for specific tables.
    /// </summary>
    public Dictionary<string, string> TableNameMappings { get; set; } = [];

    /// <summary>
    /// The default value for `consistentRead` in read operations if no value is specified.
    /// </summary>
    public bool DefaultConsistentRead { get; set; } = false;
}
