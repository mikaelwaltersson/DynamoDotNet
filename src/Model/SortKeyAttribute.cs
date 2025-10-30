namespace DynamoDB.Net.Model;

/// <summary>
/// Indicates that a property or field is used as a sort (range) key for a table
/// or index. Can be applied multiple times to participate in different indexes.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public sealed class SortKeyAttribute : IndexKeyAttribute
{
    /// <summary>
    /// The ordinal of the property when used as a local secondary index sort key.
    /// </summary>
    public int LocalSecondaryIndex
    {
        get => IndexType == IndexType.LocalSecondaryIndex ? Ordinal : -1;
        init => SetOrdinalForIndexType(IndexType.LocalSecondaryIndex, value, MaxNumberOfLocalSecondaryIndexes);
    }

    /// <summary>
    /// The ordinal of the property when used as a global secondary index sort key.
    /// </summary>
    public int GlobalSecondaryIndex
    {
        get => IndexType == IndexType.GlobalSecondaryIndex ? Ordinal : -1;
        init => SetOrdinalForIndexType(IndexType.GlobalSecondaryIndex, value, MaxNumberOfGlobalSecondaryIndexes);
    }  
}
