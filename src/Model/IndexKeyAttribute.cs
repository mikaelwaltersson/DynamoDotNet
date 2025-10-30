namespace DynamoDB.Net.Model;

/// <summary>
/// Base attribute for members that participate in table keys or indexes.
/// Provides common configuration for index ordinals and names.
/// </summary>
public abstract class IndexKeyAttribute : Attribute
{
    /// <summary>
    /// Maximum number of local secondary indexes supported per table.
    /// </summary>
    public const int MaxNumberOfLocalSecondaryIndexes = 5;

    /// <summary>
    /// Maximum number of global secondary indexes supported per table.
    /// </summary>
    public const int MaxNumberOfGlobalSecondaryIndexes = 20;

    /// <summary>
    /// Internal helper that sets the ordinal for the specified index type and validates range.
    /// </summary>
    protected void SetOrdinalForIndexType(IndexType indexType, int value, int maximum)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value, maximum);

        if (IndexType != default)
            throw new InvalidOperationException("Multiple index types specified");

        IndexType = indexType;
        Ordinal = value;
    }

    /// <summary>
    /// Optional explicit index name to use for this member when participating in an index.
    /// </summary>
    public string? IndexName { get; init; }

    /// <summary>
    /// The index type assigned to this member (primary, local secondary or global secondary).
    /// </summary>
    public IndexType IndexType { get; private set; }

    /// <summary>
    /// The ordinal position of this member within the index when applicable.
    /// </summary>
    public int Ordinal { get; private set; }
}
