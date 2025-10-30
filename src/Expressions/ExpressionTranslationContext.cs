using Amazon.DynamoDBv2.Model;
using DynamoDB.Net.Serialization;

namespace DynamoDB.Net.Expressions;

/// <summary>
/// Represents the translation context used when converting 
/// expression trees into DynamoDB expression strings.
/// </summary>
public class ExpressionTranslationContext
{
    Dictionary<string, string>? attributeNames;
    Dictionary<string, string>? attributeNameAliases;
    Dictionary<string, AttributeValue>? attributeValues;
    Dictionary<AttributeValue, string>? attributeValueAliases;

    /// <summary>
    /// Initializes a new translation context using the specified <paramref name="serializer"/>.
    /// </summary>
    /// <param name="serializer">The <see cref="IDynamoDBSerializer"/> instance to serialize item attribute values.</param>
    public ExpressionTranslationContext(IDynamoDBSerializer serializer)
    {
        ArgumentNullException.ThrowIfNull(serializer);

        Serializer = serializer;
    }

    /// <summary>
    /// The serializer used to serialize item attribute values.
    /// </summary>
    public IDynamoDBSerializer Serializer { get; }

    /// <summary>
    /// Mapping of placeholder names to DynamoDB attribute names used in the
    /// translated expression.
    /// </summary>
    public Dictionary<string, string>? AttributeNames => attributeNames;

    /// <summary>
    /// Mapping of placeholder aliases to DynamoDB attribute values used in the
    /// translated expression.
    /// </summary>
    public Dictionary<string, AttributeValue>? AttributeValues => attributeValues;

    internal Stack<DynamoDBExpressions.ArrayConstantKind> ArrayConstantKind { get; } = new([DynamoDBExpressions.ArrayConstantKind.Unspecified]);

    /// <summary>
    /// Resolve the placeholder for a item attribute name, adding a new 
    /// name to placeholder mapping to the context if it doesn't already exist.
    /// </summary>
    /// <param name="name">The name to resolve to a placeholder.</param>
    public string GetOrAddAttributeName(string name) =>
        GetOrAddWithAlias(name, ref attributeNameAliases, ref attributeNames, StringComparer.InvariantCulture, "#p");

    /// <summary>
    /// Resolve the placeholder for a <see cref="AttributeValue" />, adding a new 
    /// value to placeholder mapping to the context if it doesn't already exist.
    /// </summary>
    /// <param name="value">The value to resolve to a placeholder.</param>
    public string GetOrAddAttributeValue(AttributeValue value) =>
        GetOrAddWithAlias(value, ref attributeValueAliases, ref attributeValues, AttributeValueComparer.Default, ":v");

    static string GetOrAddWithAlias<TValue>(
        TValue value,
        ref Dictionary<TValue, string>? valueToAlias,
        ref Dictionary<string, TValue>? aliasToValue,
        IEqualityComparer<TValue> valueComparer,
        string prefix) where TValue : class
    {
        ArgumentNullException.ThrowIfNull(value);

        valueToAlias ??= new(valueComparer);

        if (!valueToAlias.TryGetValue(value, out var alias))
        {
            alias = GetNextUnusedAlias(ref aliasToValue, prefix);
            valueToAlias.Add(value, alias);
            aliasToValue!.Add(alias, value);
        }

        return alias;
    }

    static string GetNextUnusedAlias<TValue>(ref Dictionary<string, TValue>? aliasToValue, string prefix)
    {
        aliasToValue ??= new(StringComparer.Ordinal);

        for (var i = aliasToValue.Count; ; i++)
        {
            var alias = $"{prefix}{i}";
            if (!aliasToValue.ContainsKey(alias))
                return alias;
        }
    }

    /// <summary>
    /// Merges names and values from a <see cref="DynamoDBExpressions.RawExpression"/> into
    /// this context, creating aliases for any placeholders as needed.
    /// </summary>
    internal void Add(DynamoDBExpressions.RawExpression raw)
    {
        if (raw.names != null)
        {
            this.attributeNames ??= [];

            foreach (var entry in raw.names)
                this.attributeNames.Add(entry.Key, entry.Value);
        }

        if (raw.values != null)
        {
            this.attributeValues ??= [];

            foreach (var entry in raw.values)
            {
                this.attributeValues.Add(
                    entry.Key,
                    entry.Value as AttributeValue ??
                    Serializer.SerializeDynamoDBValue(entry.Value));
            }
        }
    }
}
