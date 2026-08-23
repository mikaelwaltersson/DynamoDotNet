using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;
using Amazon.DynamoDBv2.Model;
using DynamoDB.Net.Model;
using DynamoDB.Net.Serialization;
using DynamoDB.Net.Serialization.Converters;

namespace DynamoDB.Net;

/// <summary>
/// Provides helper methods for creating and inspecting <c>PrimaryKey&lt;T&gt;</c> values.
/// </summary>
public static class PrimaryKey
{
    /// <summary>
    /// The default separator character to use when converting partition / sort key pairs to a string value.
    /// </summary>
    public const char DefaultKeysSeparator = ',';

    /// <summary>
    /// Determines whether the specified <paramref name="type"/> is a <c>PrimaryKey&lt;T&gt;</c> type
    /// and returns the underlying item type if true.
    /// </summary>
    /// <param name="type">Type to inspect.</param>
    /// <param name="itemType">When the method returns true, contains the item type <c>typeof(T)</c>; otherwise null.</param>
    /// <returns><c>true</c> if <paramref name="type"/> represents a <c>PrimaryKey&lt;T&gt;</c> value, otherwise <c>false</c>.</returns>
    public static bool IsPrimaryKeyType(Type type, [NotNullWhen(true)] out Type? itemType)
    {
        ArgumentNullException.ThrowIfNull(type);

        var typeInfo = GenericTypeInfo.Get(type);

        itemType = typeInfo.PrimaryKeyItemType;
        return itemType != null;
    }

    /// <summary>
    /// Creates a PrimaryKey&lt;T&gt; instance for the given <paramref name="item"/> by
    /// reading the partition and optional sort key values using the table description accessors.
    /// </summary>
    /// <typeparam name="T">Item type.</typeparam>
    /// <param name="item">Item instance to extract keys from.</param>
    /// <returns>A PrimaryKey&lt;T&gt; representing the item's key values.</returns>
    public static PrimaryKey<T> ForItem<T>(T item) where T : class => PrimaryKey<T>.ForItem(item);
}

/// <summary>
/// Represents a primary key value of an item.
/// </summary>
/// <typeparam name="T">The item type stored in the table.</typeparam>
public readonly struct PrimaryKey<T> : IPrimaryKey, IEquatable<PrimaryKey<T>> where T : class
{
    /// <inheritdoc />
    public object PartitionKey { get; }

    /// <inheritdoc />
    public object? SortKey { get; }

    /// <inheritdoc />
    public IReadOnlyList<KeyValuePair<string, object>>? AdditionalKeyValuePairs { get; }
    
    /// <summary>
    /// Creates a PrimaryKey&lt;T&gt; from a tuple of (partitionKey, sortKey), validating types
    /// and converting values to the configured key types for <typeparamref name="T"/>.
    /// </summary>
    /// <param name="keyTuple">Tuple containing partition and (optional) sort key values.</param>
    /// <returns>A PrimaryKey&lt;T&gt; representing the provided key values.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when tuple shape doesn't match the table's key configuration.</exception>
    public static PrimaryKey<T> FromTuple((object, object?) keyTuple)
    {
        var (partitionKey, sortKey) = keyTuple;

        if (partitionKey == null || (sortKey == null) != (TableDescription.PropertyTypes<T>.SortKey == null))
            throw new ArgumentOutOfRangeException(nameof(keyTuple));

        partitionKey = CastConvert.CastTo(partitionKey, TableDescription.PropertyTypes<T>.PartitionKey)!;

        if (TableDescription.PropertyTypes<T>.SortKey != null)
            sortKey = CastConvert.CastTo(sortKey!, TableDescription.PropertyTypes<T>.SortKey);

        return new(partitionKey, sortKey);
    }

    /// <summary>
    /// Creates a new PrimaryKey&lt;T&gt; with additional key/value pairs included.
    /// </summary>
    /// <param name="additionalKeyValuePairs">The additional key/value pairs.</param>
    /// <returns>A PrimaryKey&lt;T&gt; representing the provided key values with additional key/value pairs included.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when tuple shape doesn't match the table's key configuration.</exception>
    public PrimaryKey<T> WithAdditionalKeyValuePairs(IEnumerable<KeyValuePair<string, object>> additionalKeyValuePairs) =>
        new(this, [.. 
            additionalKeyValuePairs
                .Select(pair => 
                TableDescription.PropertyTypes<T>.AdditionalIndexKeys.ContainsKey(pair.Key) 
                    ? pair
                    : throw new ArgumentOutOfRangeException(nameof(additionalKeyValuePairs), $"Invalid secondary index key name: '{pair.Key}'")
                )
        ]);

    /// <summary>
    /// Creates a PrimaryKey&lt;T&gt; for the provided <paramref name="item"/> by using
    /// the configured property accessors for partition and optional sort keys.
    /// </summary>
    /// <param name="item">Item to extract key values from.</param>
    /// <returns>A PrimaryKey&lt;T&gt; containing the key values for the item.</returns>
    public static PrimaryKey<T> ForItem(T item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return new(
            TableDescription.PropertyAccessors<T>.GetPartitionKey(item),
            TableDescription.PropertyAccessors<T>.GetSortKey?.Invoke(item));
    }

    /// <summary>
    /// Implicitly creates a <c>PrimaryKey&lt;T&gt;</c> value from a partition / sort key tuple.
    /// </summary>
    /// <param name="keyTuple">Tuple containing partition and optional sort key values.</param>
    public static implicit operator PrimaryKey<T>((object, object?) keyTuple) => FromTuple(keyTuple);

    /// <summary>
    /// Equality operator for <c>PrimaryKey&lt;T&gt;</c> values.
    /// </summary>
    public static bool operator ==(PrimaryKey<T> x, PrimaryKey<T> y) => x.Equals(y);

    /// <summary>
    /// Inequality operator for <c>PrimaryKey&lt;T&gt;</c> values.
    /// </summary>
    public static bool operator !=(PrimaryKey<T> x, PrimaryKey<T> y) => !(x == y);

    /// <inheritdoc />
    public bool Equals(PrimaryKey<T> other) =>
        KeyEquals(this.PartitionKey, other.PartitionKey) &&
        KeyEquals(this.SortKey, other.SortKey);

    /// <inheritdoc />
    public override bool Equals(object? other) =>
        other is PrimaryKey<T> key && 
        Equals(key);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = 0;

        if (PartitionKey != null)
            hashCode = KeyHashCode(PartitionKey);

        if (SortKey != null)
            hashCode = (((hashCode << 5) + hashCode) ^ KeyHashCode(SortKey));

        return hashCode;
    }

    /// <inheritdoc />
    public override string ToString() => ToString(null);

    /// <summary>
    /// Converts the primary key value to a <c>string</c>.
    /// </summary>
    /// <param name="serializer">The serializer instance to use if other than the default instance.</param>
    /// <param name="keysSeparator">The separator character to use in case for partition / sort key pairs, defaults to <c>','</c>.</param>
    /// <returns>The string representation of the primary key.</returns>
    public string ToString(IDynamoDBSerializer? serializer = null, char keysSeparator = PrimaryKey.DefaultKeysSeparator)
    {
        var s = new StringBuilder();

        serializer ??= DynamoDBSerializer.Default;

        s.Append(
            EscapeKeyValue(
                serializer.SerializeDynamoDBValue(this.PartitionKey, TableDescription.PropertyTypes<T>.PartitionKey),
                keysSeparator));

        if (TableDescription.PropertyTypes<T>.SortKey != null)
        {
            s.Append(keysSeparator);
            s.Append(
                EscapeKeyValue(
                    serializer.SerializeDynamoDBValue(this.SortKey, TableDescription.PropertyTypes<T>.SortKey),
                    keysSeparator));
        }

        if (this.AdditionalKeyValuePairs != null)
        {
            foreach (var pair in this.AdditionalKeyValuePairs)
            {
                s.Append(keysSeparator);
                s.Append(EscapeKeyValue(pair.Key, keysSeparator));
                s.Append(keysSeparator);
                s.Append(
                    EscapeKeyValue(
                        serializer.SerializeDynamoDBValue(pair.Value, TableDescription.PropertyTypes<T>.AdditionalIndexKeys[pair.Key]),
                        keysSeparator));
            }
        }

        return s.ToString();
    }

    /// <summary>
    /// Parses a string representation of a primary key into a <c>PrimaryKey&lt;T&gt;</c> value.
    /// </summary>
    /// <param name="s">The string representation of the primary key.</param>
    /// <param name="serializer">The serializer instance to use if other than the default instance.</param>
    /// <param name="keysSeparator">The separator character to use in case for partition / sort key pairs, defaults to <c>','</c>.</param>
    /// <returns>The parsed PrimaryKey&lt;T&gt; value.</returns>
    public static PrimaryKey<T> Parse(string s, IDynamoDBSerializer? serializer = null, char keysSeparator = PrimaryKey.DefaultKeysSeparator)
    {
        var key = s.Split(keysSeparator);
        var keyLength = (TableDescription.PropertyTypes<T>.SortKey != null ? 2 : 1);
        
        if (key.Length % 2 != keyLength % 2)
            throw new FormatException(nameof(s));

        serializer ??= DynamoDBSerializer.Default;

        var primaryKey =
            new PrimaryKey<T>(
                DeserializeKeyValue(serializer, UnescapeKeyValue(key[0]), TableDescription.PropertyTypes<T>.PartitionKey),
                TableDescription.PropertyTypes<T>.SortKey != null
                    ? DeserializeKeyValue(serializer, UnescapeKeyValue(key[1]), TableDescription.PropertyTypes<T>.SortKey)
                    : null);

        if (key.Length > keyLength)
        {
            primaryKey = primaryKey.WithAdditionalKeyValuePairs(
                from pair in key.Skip(keyLength).Chunk(2)
                let name = UnescapeKeyValue(pair.ElementAt(0)) 
                let value = DeserializeKeyValue(serializer, UnescapeKeyValue(pair.ElementAt(1)), TableDescription.PropertyTypes<T>.AdditionalIndexKeys[name])
                select new KeyValuePair<string, object>(name, value)
            );
        }

        return primaryKey;
    }

    PrimaryKey(object partitionKey, object? sortKey)
    {
        PartitionKey = partitionKey;
        SortKey = sortKey;
    }

    PrimaryKey(PrimaryKey<T> primaryKey, KeyValuePair<string, object>[] additionalKeyValuePairs)
    {
        PartitionKey = primaryKey.PartitionKey;
        SortKey = primaryKey.SortKey;
        AdditionalKeyValuePairs = additionalKeyValuePairs;
    }

    static object DeserializeKeyValue(IDynamoDBSerializer serializer, string value, Type type) =>
        serializer.DeserializeDynamoDBValue(
            type == typeof(byte[])
                ? new() { B = new(Convert.FromBase64String(value)) }
                : DynamoDBNumber.IsSupportedType(type)
                    ? new() { N = value }
                    : new() { S = value },
            type)!;

    static string UnescapeKeyValue(string s) => Uri.UnescapeDataString(s);

    static string EscapeKeyValue(string s, char keyValueSeparator)
    {
        var escaped = (StringBuilder?)null;
        for (var i = 0; i < s.Length; i++)
        {
            var c = s[i];
            if (c == '%' || c == keyValueSeparator)
            {
                escaped ??= new StringBuilder(s, 0, i, s.Length + 1);
                escaped.Append($"%{((int)c):X2}");
            }
            else
                escaped?.Append(c);
        }
        return escaped?.ToString() ?? s;
    }

    static string EscapeKeyValue(AttributeValue value, char keyValueSeparator) =>
        EscapeKeyValue(value.B?.ToBase64String() ?? value.N ?? value.S ?? string.Empty, keyValueSeparator);

    static bool KeyEquals(object? x, object? y)
    {
        if (Equals(x, y))
            return true;

        if (x is byte[] xByteArray && y is byte[] yByteArray)
            return ByteArrayComparer.Default.Equals(xByteArray,  yByteArray);

        return false;
    }

    static int KeyHashCode(object obj)
    {
        if (obj is byte[] byteArray)
            return ByteArrayComparer.Default.GetHashCode(byteArray);

        return obj.GetHashCode();
    }

    static class CastConvert
    {
        static readonly ConcurrentDictionary<(Type, Type), Func<object?, object?>> compiledCastTo = new();

        static Func<object?, object?> CompileCastTo((Type, Type) types)
        {
            var (fromType, toType) = types;

            var parameter = Expression.Parameter(typeof(object), "value");

            var compiledCast = 
                Expression
                    .Lambda<Func<object?, object?>>(
                        Enumerable.Aggregate(
                            [fromType, toType, typeof(object)], 
                            (Expression)parameter, 
                            Expression.Convert), 
                        parameter)
                    .Compile();

            return compiledCast;
        }

        public static object? CastTo(object value, Type type) =>
            type.IsAssignableFrom(value.GetType()) 
                ? value
                : compiledCastTo.GetOrAdd((value.GetType(), type), CompileCastTo)(value);
    }
}
