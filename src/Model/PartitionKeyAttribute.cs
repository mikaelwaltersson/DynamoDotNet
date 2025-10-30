namespace DynamoDB.Net.Model;

/// <summary>
/// Marks a property or field as the partition (hash) key for a table or index.
/// Can be applied multiple times to participate in different global secondary indexes.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public sealed class PartitionKeyAttribute : IndexKeyAttribute
{
    /// <summary>
    /// The ordinal of the property when used as a global secondary index partition key.
    /// </summary>
    public int GlobalSecondaryIndex
    {
        get => IndexType == IndexType.GlobalSecondaryIndex ? Ordinal : -1;
        init => SetOrdinalForIndexType(IndexType.GlobalSecondaryIndex, value, MaxNumberOfGlobalSecondaryIndexes);
    }  
}
