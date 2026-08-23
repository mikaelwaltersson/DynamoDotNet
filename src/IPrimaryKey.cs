using DynamoDB.Net.Serialization;

namespace DynamoDB.Net;

/// <summary>
/// Represents a primary key value of an item.
/// </summary>
public interface IPrimaryKey
{
    /// <summary>
    /// The partition key value.
    /// </summary>
    object PartitionKey { get; }

    /// <summary>
    /// The sort key value, if the table has one.
    /// </summary>
    object? SortKey { get; }

    /// <summary>
    /// Additional secondary index key values, used for <c>LastEvaluatedKey</c> and <c>ExclusiveStartKey</c>.
    /// </summary>
    IReadOnlyList<KeyValuePair<string, object>>? AdditionalKeyValuePairs { get; }

    /// <summary>
    /// Converts the primary key value to a <c>string</c>.
    /// </summary>
    /// <param name="serializer">The serializer instance to use if other than the default instance.</param>
    /// <param name="keysSeparator">The separator character to use in case for partition / sort key pairs, defaults to <c>','</c>.</param>
    /// <returns>The string representation of the primary key.</returns>
    string ToString(IDynamoDBSerializer? serializer = null, char keysSeparator = PrimaryKey.DefaultKeysSeparator);
}