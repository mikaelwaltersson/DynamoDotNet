using System.Collections;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace DynamoDB.Net;

/// <summary>
/// Overloads for methods on the <see cref="IDynamoDBClient" /> interface.
/// </summary>
public static class DynamoDBClientExtensions
{
    /// <summary>
    /// Retrieves a single item by primary key, throws <see cref="ItemNotFoundException" /> if the item does not exist.
    /// </summary>
    /// <param name="client">The <see cref="IDynamoDBClient" /> instance.</param>
    /// <param name="key">The primary key of the item to retrieve.</param>
    /// <param name="consistentRead">If <c>true</c>, performs a strongly consistent read. If <c>null</c>, uses the table default.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>The retrieved item.</returns>
    public static Task<object> GetAsync(
        this IDynamoDBClient client,
        IPrimaryKey key,
        bool? consistentRead = false,
        CancellationToken cancellationToken = default) =>
        ItemOperationsForKeyType(key).GetAsync(client, key, consistentRead, cancellationToken);

    /// <summary>
    /// Attempts to retrieve a single item by primary key, returns null if the item does not exist.
    /// </summary>
    /// <param name="client">The <see cref="IDynamoDBClient" /> instance.</param>
    /// <param name="key">The primary key of the item to retrieve.</param>
    /// <param name="consistentRead">If <c>true</c>, performs a strongly consistent read. If <c>null</c>, uses the table default.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>The retrieved item, or <c>null</c> if not found.</returns>
    public static Task<object?> TryGetAsync(
        this IDynamoDBClient client,
        IPrimaryKey key,
        bool? consistentRead = false,
        CancellationToken cancellationToken = default) =>
        ItemOperationsForKeyType(key).TryGetAsync(client, key, consistentRead, cancellationToken);

    /// <summary>
    /// Puts (inserts or replaces) an item into the table.
    /// </summary>
    /// <param name="client">The <see cref="IDynamoDBClient" /> instance.</param>
    /// <param name="item">The item to be stored in the table.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>The stored item.</returns>
    public static Task<object> PutAsync(
        this IDynamoDBClient client,
        object item,
        CancellationToken cancellationToken = default) =>
        ItemOperationsForEntityType(item.GetType()).PutAsync(client, item, cancellationToken);

    /// <summary>
    /// Performs a table scan across all item with optional filter expression and returns a partial, paginated result set of items.
    /// </summary>
    /// <param name="client">The <see cref="IDynamoDBClient" /> instance.</param>
    /// <param name="entityType">The item type stored in the table.</param>
    /// <param name="exclusiveStartKey">Optional primary key to start scanning from (for pagination).</param>
    /// <param name="limit">Optional maximum number of items to return in this page.</param>
    /// <param name="consistentRead">If <c>true</c>, performs a strongly consistent read. If <c>null</c>, uses the table default.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A partial, paginated result set of items.</returns>
    public static Task<IDynamoDBPartialResult> ScanAsync(
        this IDynamoDBClient client,
        Type entityType,
        IPrimaryKey? exclusiveStartKey = null,
        int? limit = null,
        bool consistentRead = false,
        CancellationToken cancellationToken = default) =>
        ItemOperationsForEntityType(entityType).ScanAsync(client, exclusiveStartKey, limit, consistentRead, cancellationToken);

    /// <summary>
    /// Performs a table scan across all item with optional filter expression and returns all remaining items using multiple requests.
    /// </summary>
    /// <typeparam name="T">The item type stored in the table.</typeparam>
    /// <param name="client">The <see cref="IDynamoDBClient" /> instance.</param>
    /// <param name="filter">Optional filter expression applied to scanned items.</param>
    /// <param name="exclusiveStartKey">Optional primary key to start scanning from (for pagination).</param>
    /// <param name="limit">Optional maximum number of items to return.</param>
    /// <param name="consistentRead">If <c>true</c>, performs a strongly consistent read. If <c>null</c>, uses the table default.</param>
    /// <param name="indexProperties">Optional index to scan instead of the base table.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A result set of all remaing items to scan.</returns>
    public static Task<IReadOnlyList<T>> ScanRemainingAsync<T>(
        this IDynamoDBClient client,
        Expression<Func<T, bool>>? filter = null,
        PrimaryKey<T> exclusiveStartKey = default,
        int? limit = null,
        bool consistentRead = false,
        (string?, string?) indexProperties = default,
        CancellationToken cancellationToken = default) where T : class =>
        RemainingAsync(
            new List<T>(),
            exclusiveStartKey,
            limit,
            (lastEvaluatedKey, remainingLimit) => client.ScanAsync(filter, lastEvaluatedKey, remainingLimit, consistentRead, indexProperties, cancellationToken));

    /// <summary>
    /// Performs a table scan across all item with optional filter expression and returns all remaining items using multiple requests.
    /// </summary>
    /// <param name="entityType">The item type stored in the table.</param>
    /// <param name="client">The <see cref="IDynamoDBClient" /> instance.</param>
    /// <param name="exclusiveStartKey">Optional primary key to start scanning from (for pagination).</param>
    /// <param name="limit">Optional maximum number of items to return.</param>
    /// <param name="consistentRead">If <c>true</c>, performs a strongly consistent read. If <c>null</c>, uses the table default.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A result set of all remaing items to scan.</returns>
    public static Task<IReadOnlyList<object>> ScanRemainingAsync(
        this IDynamoDBClient client,
        Type entityType,
        IPrimaryKey? exclusiveStartKey = null,
        int? limit = null,
        bool consistentRead = false,
        CancellationToken cancellationToken = default) =>
        RemainingAsync(
            new List<object>(),
            exclusiveStartKey,
            limit,
            (lastEvaluatedKey, remainingLimit) => client.ScanAsync(entityType, lastEvaluatedKey, remainingLimit, consistentRead, cancellationToken));

    /// <summary>
    /// Queries the table using a key condition and optional filter, returns all remaining items using multiple requests.
    /// </summary>
    /// <typeparam name="T">The item type stored in the table.</typeparam>
   /// <param name="client">The <see cref="IDynamoDBClient" /> instance.</param>
    /// <param name="keyCondition">A key condition expression that restricts the query to a partition key (and optionally sort key range).</param>
    /// <param name="filter">Optional filter expression applied in addition to the key condition.</param>
    /// <param name="exclusiveStartKey">Optional primary key to start the query from (for pagination).</param>
    /// <param name="scanIndexForward">If <c>true</c>, results are returned in ascending sort key order; if <c>false</c>, descending.</param>
    /// <param name="limit">Optional maximum number of items to return.</param>
    /// <param name="consistentRead">If <c>true</c>, performs a strongly consistent read. If <c>null</c>, uses the table default.</param>
    /// <param name="indexProperties">Optional explicit index to query instead of automatically inferring it from the key condition expression.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A result set of all remaing items matching the query.</returns>
    public static Task<IReadOnlyList<T>> QueryRemainingAsync<T>(
        this IDynamoDBClient client,
        Expression<Func<T, bool>> keyCondition,
        Expression<Func<T, bool>>? filter = null,
        PrimaryKey<T> exclusiveStartKey = default,
        bool? scanIndexForward = null,
        int? limit = null,
        bool? consistentRead = false,
        (string?, string?) indexProperties = default,
        CancellationToken cancellationToken = default) where T : class =>
        RemainingAsync(
            new List<T>(),
            exclusiveStartKey,
            limit,
            (lastEvaluatedKey, remainingLimit) => client.QueryAsync(keyCondition, filter, lastEvaluatedKey, scanIndexForward, remainingLimit, consistentRead, indexProperties, cancellationToken));


    async static Task<IReadOnlyList<T>> RemainingAsync<T, TKey, TPartialResult>(List<T> result, TKey? exclusiveStartKey, int? limit, Func<TKey?, int?, Task<TPartialResult>> next)
        where T : class
        where TKey : IPrimaryKey
        where TPartialResult : IDynamoDBPartialResult<T, TKey>
    {
        var lastEvaluatedKey = exclusiveStartKey;

        do
        {
            var partialResult = await next(lastEvaluatedKey, limit);

            result.AddRange(partialResult);
            lastEvaluatedKey = partialResult.LastEvaluatedKey;

            if (limit.HasValue)
            {
                limit -= partialResult.Count;
                if (!(limit > 0))
                    break;
            }
        }
        while (lastEvaluatedKey.PartitionKey != null);

        return result;
    }



    static readonly ConcurrentDictionary<Type, IItemOperations> itemOperations = [];

    static IItemOperations ItemOperationsForKeyType(IPrimaryKey key) =>
        itemOperations.GetOrAdd(
            key.GetType(),
            static type =>
            {
                if (!PrimaryKey.IsPrimaryKeyType(type, out var itemType))
                    throw new ArgumentOutOfRangeException(nameof(key));

                return (IItemOperations)Serialization.Activator.CreateInstance(typeof(ItemOperations<>).MakeGenericType(itemType));
            });

    static IItemOperations ItemOperationsForEntityType(Type entityType) =>
        itemOperations.GetOrAdd(
            entityType,
            type =>
            {
                if (type == null)
                    throw new ArgumentOutOfRangeException(nameof(entityType));

                return (IItemOperations)Serialization.Activator.CreateInstance(typeof(ItemOperations<>).MakeGenericType(type));
            });


    interface IItemOperations
    {
        Task<object> GetAsync(IDynamoDBClient client, IPrimaryKey key, bool? consistentRead, CancellationToken cancellationToken);

        Task<object?> TryGetAsync(IDynamoDBClient client, IPrimaryKey key, bool? consistentRead, CancellationToken cancellationToken);

        Task<object> PutAsync(IDynamoDBClient client, object item, CancellationToken cancellationToken);

        Task<IDynamoDBPartialResult> ScanAsync(IDynamoDBClient client, IPrimaryKey? exclusiveStartKey, int? limit, bool? consistentRead, CancellationToken cancellationToken);
    }

    class ItemOperations<T> : IItemOperations where T : class
    {
        public async Task<object> GetAsync(IDynamoDBClient client, IPrimaryKey key, bool? consistentRead, CancellationToken cancellationToken) =>
            await client.GetAsync((PrimaryKey<T>)key, consistentRead, cancellationToken);

        public async Task<object?> TryGetAsync(IDynamoDBClient client, IPrimaryKey key, bool? consistentRead, CancellationToken cancellationToken) =>
            await client.TryGetAsync((PrimaryKey<T>)key, consistentRead, cancellationToken);

        public async Task<object> PutAsync(IDynamoDBClient client, object item, CancellationToken cancellationToken) =>
            await client.PutAsync((T)item, (Expression<Func<T, bool>>?)null, cancellationToken);

        public async Task<IDynamoDBPartialResult> ScanAsync(IDynamoDBClient client, IPrimaryKey? exclusiveStartKey, int? limit, bool? consistentRead, CancellationToken cancellationToken) =>
            new DynamoDBPartialResult(await client.ScanAsync(null, ((PrimaryKey<T>?)exclusiveStartKey).GetValueOrDefault(), limit, consistentRead, (null, null), cancellationToken));

        class DynamoDBPartialResult : IDynamoDBPartialResult
        {
            IDynamoDBPartialResult<T> result;

            public DynamoDBPartialResult(IDynamoDBPartialResult<T> result)
            {
                this.result = result;
            }

            public object this[int index] => result[index];

            public int Count => result.Count;

            public IPrimaryKey LastEvaluatedKey => result.LastEvaluatedKey;

            public IEnumerator<object> GetEnumerator() => result.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => result.GetEnumerator();
        }
    }
}
