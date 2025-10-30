using System.Text.Json;
using System.Text.Json.Nodes;
using Amazon.DynamoDBv2.Model;

namespace DynamoDB.Net.Serialization.Converters;

/// <summary>
/// Type converter for serializing and deserializing <see cref="JsonNode" /> values.
/// </summary>
public class JsonNodeTypeConverter
    : DynamoDBTypeConverter,
    IConvertFromNull, IConvertFromBoolean,
    IConvertFromString, IConvertFromNumber, IConvertFromBinary,
    IConvertFromStringSet, IConvertFromNumberSet, IConvertFromBinarySet,
    IConvertFromList,
    IConvertFromMap,
    IConvertToDynamoDBValue
{
    /// <inheritdoc />
    public override bool Handle(Type type) =>
        typeof(JsonNode).IsAssignableFrom(type);

    /// <inheritdoc />
    public object? ConvertFromNull(Type toType) =>
        null;

    /// <inheritdoc />
    public object ConvertFromBoolean(bool value, Type toType) =>
        TypeAssert.ForValue(JsonValue.Create(value), toType);

    /// <inheritdoc />
    public object ConvertFromString(string value, Type toType) =>
        TypeAssert.ForValue(JsonValue.Create(value), toType);

    /// <inheritdoc />
    public object ConvertFromNumber(string value, Type toType) =>
        TypeAssert.ForValue(JsonNode.Parse(value)!, toType);

    /// <inheritdoc />
    public object ConvertFromBinary(MemoryStream value, Type toType) =>
        TypeAssert.ForValue(JsonValue.Create(value.ToBase64String()), toType);

    /// <inheritdoc />
    public object ConvertFromStringSet(ICollection<string> values, Type toType, IConvertFromString convertFromString) =>
        TypeAssert.ForValue(new JsonArray(values.Select(value => JsonValue.Create(value)).ToArray()), toType);

    /// <inheritdoc />
    public object ConvertFromNumberSet(ICollection<string> values, Type toType, IConvertFromNumber convertFromNumber) =>
        TypeAssert.ForValue(new JsonArray(values.Select(value => JsonNode.Parse(value)).ToArray()), toType);

    /// <inheritdoc />
    public object ConvertFromBinarySet(ICollection<MemoryStream> values, Type toType, IConvertFromBinary convertFromBinary) =>
        TypeAssert.ForValue(new JsonArray(values.Select(value => JsonValue.Create(value.ToBase64String())).ToArray()), toType);

    /// <inheritdoc />
    public object ConvertFromList(List<AttributeValue> elements, Type toType, IDynamoDBSerializer serializer) =>
        TypeAssert.ForValue(new JsonArray(elements.Select(serializer.DeserializeDynamoDBValue<JsonNode>).ToArray()), toType);

    /// <inheritdoc />
    public object ConvertFromMap(Dictionary<string, AttributeValue> entries, Type toType, IDynamoDBSerializer serializer) =>
       TypeAssert.ForValue(new JsonObject(entries.Select(entry => new KeyValuePair<string, JsonNode?>(entry.Key, serializer.DeserializeDynamoDBValue<JsonNode>(entry.Value)))), toType);

    /// <inheritdoc />
    public AttributeValue ConvertToDynamoDBValue(object? value, Type fromType, IDynamoDBSerializer serializer) =>
        value switch
        {
            null => new() { NULL = true },

            JsonValue jsonValue => jsonValue.GetValueKind() switch
            {
                JsonValueKind.False => new() { BOOL = false },

                JsonValueKind.True => new() { BOOL = true },

                JsonValueKind.String => new() { S = jsonValue.GetValue<string>() },

                JsonValueKind.Number => new() { N = jsonValue.ToJsonString() },

                _ => new()
            },

            JsonArray jsonArray =>
                new()
                {
                    L = new(jsonArray.Select(serializer.SerializeDynamoDBValue)),
                    IsLSet = true
                },

            JsonObject jsonObject =>
                new()
                {
                    M = new(jsonObject.Select(entry => KeyValuePair.Create(entry.Key, serializer.SerializeDynamoDBValue(entry.Value)))),
                    IsMSet = true
                },

            _ => new()
        };
}
