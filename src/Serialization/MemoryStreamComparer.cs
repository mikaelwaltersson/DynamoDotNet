namespace DynamoDB.Net.Serialization;

/// <summary>
/// Equality comparer for <see cref="MemoryStream" /> instances.
/// </summary>
public class MemoryStreamComparer : IEqualityComparer<MemoryStream>
{
    /// <summary>
    /// The default <see cref="MemoryStreamComparer" /> instance.
    /// </summary>
    public static MemoryStreamComparer Default { get; } = new MemoryStreamComparer();

    /// <inheritdoc />
    public bool Equals(MemoryStream? x, MemoryStream? y) =>
        (x == null && y == null) ||
        (x != null && y != null && BufferOf(x).SequenceEqual(BufferOf(y)));

    /// <inheritdoc />
    public int GetHashCode(MemoryStream obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        return BufferOf(obj).SequenceCombinedHashCode();
    }

    static ArraySegment<byte> BufferOf(MemoryStream stream) =>
        stream.TryGetBuffer(out var buffer)
            ? buffer
            : new ArraySegment<byte>(stream.ToArray());
}
