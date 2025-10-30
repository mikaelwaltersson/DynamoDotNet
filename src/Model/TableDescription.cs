using System.Collections.Concurrent;
using System.Reflection;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using DynamoDB.Net.Serialization;

namespace DynamoDB.Net.Model;

/// <summary>
/// Describes table metadata inferred from a type, including table name,
/// key members and index key mappings used to build DynamoDB requests.
/// </summary>
public class TableDescription
{
    const int DefaultReadCapacityUnits = 1;

    const int DefaultWriteCapacityUnits = 1;


    TableDescription(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        TableName = GetTableName(type);
        PartitionKeyProperty = GetIndexKeyProperty<PartitionKeyAttribute>(type, required: true)!;
        SortKeyProperty = GetIndexKeyProperty<SortKeyAttribute>(type);
        VersionProperty = GetVersionProperty(type);
        LocalSecondaryIndexSortKeyProperties = GetSecondaryIndexKeyProperties<SortKeyAttribute>(type, IndexType.LocalSecondaryIndex);
        GlobalSecondaryIndexSortKeyProperties = GetSecondaryIndexKeyProperties<SortKeyAttribute>(type, IndexType.GlobalSecondaryIndex);
        GlobalSecondaryIndexPartitionKeyProperties = GetSecondaryIndexKeyProperties<PartitionKeyAttribute>(type, IndexType.GlobalSecondaryIndex, GlobalSecondaryIndexSortKeyProperties);
    }

    /// <summary>
    /// The resolved DynamoDB table name for the type.
    /// </summary>
    public string TableName { get; }

    /// <summary>
    /// The member that maps to the partition (hash) key for the table.
    /// </summary>
    public MemberInfo PartitionKeyProperty { get; }

    /// <summary>
    /// The member that maps to the optional sort (range) key for the table, or null
    /// if the table has no sort key.
    /// </summary>
    public MemberInfo? SortKeyProperty { get; }

    /// <summary>
    /// The member used to store optimistic-concurrency version information, if any.
    /// </summary>
    public MemberInfo? VersionProperty { get; }

    /// <summary>
    /// Array of sort key members used for local secondary indexes, entries may be null
    /// when a particular index does not define a sort key.
    /// </summary>
    public MemberInfo?[] LocalSecondaryIndexSortKeyProperties { get; }

    /// <summary>
    /// Array of partition key members used for global secondary indexes.
    /// </summary>
    public MemberInfo?[] GlobalSecondaryIndexPartitionKeyProperties { get; }

    /// <summary>
    /// Array of sort key members used for global secondary indexes, entries may be null
    /// when a particular global index does not define a sort key.
    /// </summary>
    public MemberInfo?[] GlobalSecondaryIndexSortKeyProperties { get; }

    static readonly ConcurrentDictionary<Type, TableDescription> cachedTableDescriptions = [];

    /// <summary>
    /// Returns the <see cref="TableDescription"/> for the given type.
    /// </summary>
    /// <param name="type">The type to get the <see cref="TableDescription"/> for.</param>
    public static TableDescription Get(Type type) =>
        cachedTableDescriptions.GetOrAdd(type, static type => new(type));

    /// <summary>
    /// Resolve the table name for a type.
    /// </summary>
    /// <typeparam name="T">The type to resolve the table name for.</typeparam>
    /// <param name="options">The <see cref="DynamoDBClientOptions"/> to use when resolving the table name.</param>
    public static string GetTableName<T>(DynamoDBClientOptions? options = null) => GetTableName(typeof(T), options);

    /// <summary>
    /// Resolve the table name for a type.
    /// </summary>
    /// <param name="type">The type to resolve the table name for.</param>
    /// <param name="options">The <see cref="DynamoDBClientOptions"/> to use when resolving the table name.</param>
    public static string GetTableName(Type type, DynamoDBClientOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(type);

        var tableAttribute =
            type.GetCustomAttribute<TableAttribute>(inherit: true) ??
            throw new InvalidOperationException($"Type {type.Name} is missing a Table attribute");

        var tableName = tableAttribute.TableName ?? type.Name.ToHyphenCase().NaivelyPluralized();

        return ApplyTableNamePrefixAndMapping(options, tableName);
    }

    /// <summary>
    /// Determines the index name that corresponds to the supplied partition and
    /// optional sort key members. Returns null for the base table primary key.
    /// </summary>
    /// <param name="partitionKey">The partition key member.</param>
    /// <param name="sortKey">The sort key member or null.</param>
    public string? GetIndexName(MemberInfo partitionKey, MemberInfo? sortKey)
    {
        ArgumentNullException.ThrowIfNull(partitionKey);

        var partitionKeyAttributes =
            partitionKey
                .GetCustomAttributes<PartitionKeyAttribute>()
                .OrderBy(attribute => attribute.IndexType)
                .ThenBy(attribute => attribute.Ordinal)
                .ToArray();

        if (partitionKeyAttributes.Length == 0)
            throw new ArgumentOutOfRangeException(
                nameof(partitionKey),
                "Not a valid primary key, local secondary index or global secondary index");

        if (sortKey == null)
        {
            return
                partitionKeyAttributes[0].IndexType switch
                {
                    IndexType.PrimaryKey =>
                        null,

                    IndexType.GlobalSecondaryIndex =>
                        GetGlobalSecondaryIndexName(partitionKeyAttributes[0].Ordinal),

                    _ =>
                        throw new ArgumentNullException(nameof(sortKey))
                };
        }

        var sortKeyAttributes =
            sortKey.GetCustomAttributes<SortKeyAttribute>()
                .OrderBy(attribute => attribute.IndexType)
                .ThenBy(attribute => attribute.Ordinal)
                .ToArray();

        if (sortKeyAttributes.Length == 0)
            throw new ArgumentOutOfRangeException(
                nameof(sortKey),
                "Not a valid primary key, local secondary index or global secondary index");

        for (var i = 0; i < partitionKeyAttributes.Length; i++)
        {
            var j =
                Array.FindIndex(
                    sortKeyAttributes,
                    attribute =>
                        (partitionKeyAttributes[i].IndexType == IndexType.PrimaryKey
                            ? (attribute.IndexType == IndexType.PrimaryKey || attribute.IndexType == IndexType.LocalSecondaryIndex)
                            : (attribute.IndexType == IndexType.GlobalSecondaryIndex && attribute.Ordinal == partitionKeyAttributes[i].Ordinal)));

            if (j >= 0)
            {
                switch (sortKeyAttributes[j].IndexType)
                {
                    case IndexType.PrimaryKey:
                        return null;

                    case IndexType.LocalSecondaryIndex:
                        return GetLocalSecondaryIndexName(sortKeyAttributes[j].Ordinal);

                    case IndexType.GlobalSecondaryIndex:
                        return GetGlobalSecondaryIndexName(sortKeyAttributes[j].Ordinal);
                }
            }
        }

        throw new ArgumentOutOfRangeException(nameof(sortKey), "Not part of the same key/index as parameter partitionKey.");
    }

    /// <summary>
    /// Builds a <see cref="CreateTableRequest"/> for this table description using
    /// the provided serializer and optional settings.
    /// </summary>
    public CreateTableRequest GetCreateTableRequest(
        IDynamoDBSerializer serializer,
        DynamoDBClientOptions? options = null,
        ProvisionedThroughput? provisionedThroughput = null,
        Projection? projection = null,
        StreamSpecification? streamSpecification = null,
        Func<Type, ScalarAttributeType>? mapToKeyAttributeType = null) =>
        TableRequests.CreateTable(this, serializer, options, provisionedThroughput, projection, streamSpecification, mapToKeyAttributeType);

    /// <summary>
    /// Builds an <see cref="UpdateTableRequest"/> that modifies the table's
    /// provisioned throughput according to the provided values and client options.
    /// </summary>
    public UpdateTableRequest GetUpdateTableProvisionedThroughputRequest(
        DynamoDBClientOptions? options = null,
        int? readCapacityUnits = null,
        int? writeCapacityUnits = null) =>
        TableRequests.UpdateTableProvisionedThroughput(this, options, readCapacityUnits, writeCapacityUnits);

    string GetLocalSecondaryIndexName(int ordinal) =>
        GetPropertyIndexAttributeName<SortKeyAttribute>(LocalSecondaryIndexSortKeyProperties[ordinal], IndexType.LocalSecondaryIndex, ordinal) ??
        $"lsi-{ordinal}-{LocalSecondaryIndexSortKeyProperties[ordinal]?.Name.ToHyphenCase()}";

    string GetGlobalSecondaryIndexName(int ordinal)
    {
        var name = GetPropertyIndexAttributeName<PartitionKeyAttribute>(GlobalSecondaryIndexPartitionKeyProperties[ordinal], IndexType.GlobalSecondaryIndex, ordinal);
        if (name != null)
            return name;

        name = $"gsi-{ordinal}-{GlobalSecondaryIndexPartitionKeyProperties[ordinal]?.Name.ToHyphenCase()}";

        if (GlobalSecondaryIndexSortKeyProperties[ordinal] != null)
            name += $"-{GlobalSecondaryIndexSortKeyProperties[ordinal]?.Name.ToHyphenCase()}";

        return name;
    }

    static IEnumerable<int> GetIndexOrdinals(IndexType type) =>
        type switch
        {
            IndexType.LocalSecondaryIndex =>
                Enumerable.Range(0, IndexKeyAttribute.MaxNumberOfLocalSecondaryIndexes),

            IndexType.GlobalSecondaryIndex =>
                Enumerable.Range(0, IndexKeyAttribute.MaxNumberOfGlobalSecondaryIndexes),

            _ => [],
        };

    static string? GetPropertyIndexAttributeName<TAttribute>(MemberInfo? property, IndexType indexType, int ordinal) where TAttribute : IndexKeyAttribute =>
        property?.GetCustomAttributes<TAttribute>().FirstOrDefault(a => a.IndexType == indexType && a.Ordinal == ordinal)?.IndexName;

    static MemberInfo? GetIndexKeyProperty<TAttribute>(Type type, IndexType indexType = IndexType.PrimaryKey, int ordinal = 0, bool required = false) where TAttribute : IndexKeyAttribute
    {
        var properties =
            type.GetSerializablePropertiesAndFields()
                .Where(p => p.GetCustomAttributes<TAttribute>().Any(a => a.IndexType == indexType && a.Ordinal == ordinal))
                .ToArray();

        var attributeDescription = typeof(TAttribute).Name;
        if (indexType != IndexType.PrimaryKey)
            attributeDescription += $"({indexType} = {ordinal})";

        return ValidSingleResolvedPropertyResult(type, properties, attributeDescription, required);
    }

    static MemberInfo? GetVersionProperty(Type type) =>
        ValidSingleResolvedPropertyResult(
            type,
            type.GetSerializablePropertiesAndFields().Where(property => property.HasCustomAttribute<VersionAttribute>()).ToArray(),
            typeof(Version).Name);

    static MemberInfo?[] GetSecondaryIndexKeyProperties<TAttribute>(Type type, IndexType indexType, MemberInfo?[]? relatedIndexProperties = null) where TAttribute : IndexKeyAttribute =>
        GetIndexOrdinals(indexType).Select((ordinal, i) => GetIndexKeyProperty<TAttribute>(type, indexType, ordinal, required: relatedIndexProperties?[i] is not null)).ToArray();

    static MemberInfo? ValidSingleResolvedPropertyResult(Type type, MemberInfo[] properties, string attributeDescription, bool required = false)
    {
        if (properties.Length > 1)
            throw new InvalidOperationException($"Expected at most one property with a {attributeDescription} attribute for {type.FullName}, got {properties.Length}");

        if (properties.Length == 0)
        {
            if (required)
                throw new InvalidOperationException($"Expected a property with a {attributeDescription} attribute for {type.FullName}");

            return null;
        }

        return properties[0];
    }

    static string ApplyTableNamePrefixAndMapping(DynamoDBClientOptions? options, string tableName) =>
        options == null
            ? tableName
            : options.TableNameMappings.TryGetValue(tableName, out var mappedName)
                ? mappedName
                : options.TableNamePrefix + tableName;


    internal static class Properties<T>
    {
        public static readonly (Type DeclaringType, string Name) PartitionKey = Get(typeof(T)).PartitionKeyProperty.AsSimplePropertyReference();

        public static readonly (Type DeclaringType, string Name)? SortKey = Get(typeof(T)).SortKeyProperty?.AsSimplePropertyReference();

        public static readonly (Type DeclaringType, string Name)? Version = Get(typeof(T)).VersionProperty?.AsSimplePropertyReference();
    }

    internal static class PropertyTypes<T>
    {
        public static readonly Type PartitionKey = Get(typeof(T)).PartitionKeyProperty.GetPropertyType();

        public static readonly Type? SortKey = Get(typeof(T)).SortKeyProperty?.GetPropertyType();

        public static readonly Type? Version = Get(typeof(T)).VersionProperty?.GetPropertyType();
    }

    internal static class PropertyAccessors<T>
    {
        public static readonly Func<T, object> GetPartitionKey = Get(typeof(T)).PartitionKeyProperty.CompilePropertyGetter<T, object>();

        public static readonly Func<T, object?>? GetSortKey = Get(typeof(T)).SortKeyProperty?.CompilePropertyGetter<T, object?>();

        public static readonly Func<T, object?>? GetVersion = Get(typeof(T)).VersionProperty?.CompilePropertyGetter<T, object?>();
    }

    static class TableRequests
    {
        public static CreateTableRequest CreateTable(
            TableDescription table,
            IDynamoDBSerializer serializer,
            DynamoDBClientOptions? options = null,
            ProvisionedThroughput? provisionedThroughput = null,
            Projection? projection = null,
            StreamSpecification? streamSpecification = null,
            Func<Type, ScalarAttributeType>? mapToKeyAttributeType = null) =>
            new()
            {
                TableName = ApplyTableNamePrefixAndMapping(options, table.TableName),
                KeySchema = GetKeySchema(serializer, table.PartitionKeyProperty, table.SortKeyProperty),
                ProvisionedThroughput = provisionedThroughput ?? GetDefaultProvisionedThrougput(),
                StreamSpecification = streamSpecification ?? GetDefaultStreamSpecification(),
                SSESpecification = new SSESpecification { Enabled = true },
                AttributeDefinitions = [.. (
                    from property in
                        new[] { table.PartitionKeyProperty, table.SortKeyProperty }
                        .Concat(table.LocalSecondaryIndexSortKeyProperties)
                        .Concat(table.GlobalSecondaryIndexPartitionKeyProperties)
                        .Concat(table.GlobalSecondaryIndexSortKeyProperties)
                    where property != null
                    group property by serializer.GetPropertyAttributeInfo(property).AttributeName into propertiesPerName
                    let property = propertiesPerName.First()
                    select new AttributeDefinition
                    {
                        AttributeName = propertiesPerName.Key,
                        AttributeType =
                            mapToKeyAttributeType?.Invoke(property.GetPropertyType()) ??
                            MapToScalarAttributeType(property.GetPropertyType())
                    })],

                LocalSecondaryIndexes = [.. (
                    from ordinal in GetIndexOrdinals(IndexType.LocalSecondaryIndex)
                    let sortKey = table.LocalSecondaryIndexSortKeyProperties[ordinal]
                    where sortKey != null
                    select new LocalSecondaryIndex
                    {
                        IndexName = table.GetLocalSecondaryIndexName(ordinal),
                        KeySchema = GetKeySchema(serializer, table.PartitionKeyProperty, sortKey),
                        Projection = projection ?? GetDefaultProjection()
                    })],

                GlobalSecondaryIndexes = [.. (
                    from ordinal in GetIndexOrdinals(IndexType.GlobalSecondaryIndex)
                    let partitionKey = table.GlobalSecondaryIndexPartitionKeyProperties[ordinal]
                    let sortKey = table.GlobalSecondaryIndexSortKeyProperties[ordinal]
                    where partitionKey != null
                    select new GlobalSecondaryIndex
                    {
                        IndexName = table.GetGlobalSecondaryIndexName(ordinal),
                        KeySchema = GetKeySchema(serializer, partitionKey, sortKey),
                        Projection = projection ?? GetDefaultProjection(),
                        ProvisionedThroughput = provisionedThroughput ?? GetDefaultProvisionedThrougput()
                    })],
            };

        public static UpdateTableRequest UpdateTableProvisionedThroughput(
            TableDescription table,
            DynamoDBClientOptions? options = null,
            int? readCapacityUnits = null,
            int? writeCapacityUnits = null) =>
            new()
            {
                TableName = ApplyTableNamePrefixAndMapping(options, table.TableName),
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = readCapacityUnits ?? DefaultReadCapacityUnits,
                    WriteCapacityUnits = writeCapacityUnits ?? DefaultWriteCapacityUnits,
                },
                GlobalSecondaryIndexUpdates = [.. (
                    from ordinal in GetIndexOrdinals(IndexType.GlobalSecondaryIndex)
                    let partitionKey = table.GlobalSecondaryIndexPartitionKeyProperties[ordinal]
                    let sortKey = table.GlobalSecondaryIndexSortKeyProperties[ordinal]
                    where partitionKey != null
                    select new GlobalSecondaryIndexUpdate
                    {
                        Update = new UpdateGlobalSecondaryIndexAction
                        {
                            IndexName = table.GetGlobalSecondaryIndexName(ordinal),
                            ProvisionedThroughput = new ProvisionedThroughput
                            {
                                ReadCapacityUnits = readCapacityUnits ?? DefaultReadCapacityUnits,
                                WriteCapacityUnits = writeCapacityUnits ?? DefaultWriteCapacityUnits,
                            }
                        }
                    })],
            };


        static List<KeySchemaElement> GetKeySchema(IDynamoDBSerializer serializer, MemberInfo partitionKeyProperty, MemberInfo? sortKeyProperty)
        {
            var elements =
                new List<KeySchemaElement>
                {
                    new()
                    {
                        AttributeName = serializer.GetPropertyAttributeInfo(partitionKeyProperty).AttributeName,
                        KeyType = KeyType.HASH
                    }
                };

            if (sortKeyProperty != null)
            {
                elements.Add(
                    new()
                    {
                        AttributeName = serializer.GetPropertyAttributeInfo(sortKeyProperty).AttributeName,
                        KeyType = KeyType.RANGE
                    });
            }

            return elements;
        }

        static ProvisionedThroughput GetDefaultProvisionedThrougput() =>
            new()
            {
                ReadCapacityUnits = DefaultReadCapacityUnits,
                WriteCapacityUnits = DefaultWriteCapacityUnits
            };

        static Projection GetDefaultProjection() =>
            new()
            {
                ProjectionType = ProjectionType.ALL
            };

        static StreamSpecification GetDefaultStreamSpecification() =>
            new()
            {
                StreamEnabled = true,
                StreamViewType = StreamViewType.NEW_AND_OLD_IMAGES
            };


        static ScalarAttributeType MapToScalarAttributeType(Type type)
        {
            type = type.UnwrapNullableType();

            return Type.GetTypeCode(type) switch
            {
                TypeCode.SByte or
                TypeCode.Byte or
                TypeCode.Int16 or
                TypeCode.UInt16 or
                TypeCode.Int32 or
                TypeCode.UInt32 or
                TypeCode.Int64 or
                TypeCode.UInt64 or
                TypeCode.Single or
                TypeCode.Double or
                TypeCode.Decimal =>
                    ScalarAttributeType.N,

                _ =>
                    type == typeof(byte[])
                        ? ScalarAttributeType.B
                        : ScalarAttributeType.S
            };
        }
    }
}
