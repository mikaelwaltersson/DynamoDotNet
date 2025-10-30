namespace DynamoDB.Net.Model;

/// <summary>
/// Enumeration of the kinds of index a key can belong to: primary key, local secondary index,
/// or global secondary index.
/// </summary>
public enum IndexType
{
    /// <summary>
    /// The member is part of the table's primary key.
    /// </summary>
    PrimaryKey,

    /// <summary>
    /// The member is used as a local secondary index (LSI) sort key.
    /// </summary>
    LocalSecondaryIndex,

    /// <summary>
    /// The member is used for a global secondary index (GSI).
    /// </summary>
    GlobalSecondaryIndex
}
