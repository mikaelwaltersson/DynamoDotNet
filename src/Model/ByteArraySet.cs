namespace DynamoDB.Net.Model;

/// <summary>
/// Specialized version of a sorted set of byte arrays for representing a DynamoDB 'BS' type.
/// </summary>
public class ByteArraySet : SortedSet<byte[]>, IEquatable<ByteArraySet>
{
    /// <inheritdoc />
    public ByteArraySet() : base(ByteArrayComparer.Default)
    {
    }

    /// <inheritdoc />
    public ByteArraySet(IEnumerable<byte[]> collection) : base(collection, ByteArrayComparer.Default)
    {
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) =>
        obj is ByteArraySet other && Equals(other);

    /// <inheritdoc />
    public bool Equals(ByteArraySet? other) =>
        other != null && this.SequenceEqual(other, ByteArrayComparer.Default);

    /// <inheritdoc />
    public override int GetHashCode() =>
        this.SequenceCombinedHashCode(ByteArrayComparer.Default);
}
