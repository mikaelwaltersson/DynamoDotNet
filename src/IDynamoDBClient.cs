using System.Linq.Expressions;
using DynamoDB.Net.Expressions;

namespace DynamoDB.Net;

/// <summary>
/// Represents a client for performing operations against DynamoDB.
/// </summary>
public interface IDynamoDBClient
{
    /// <summary>
    /// Retrieves a single item by primary key, throws <see cref="ItemNotFoundException{T}" /> if the item does not exist.
    /// </summary>
    /// <typeparam name="T">The item type stored in the table.</typeparam>
    /// <param name="key">The primary key of the item to retrieve.</param>
    /// <param name="consistentRead">If <c>true</c>, performs a strongly consistent read. If <c>null</c>, uses the table default.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>The retrieved item of type <typeparamref name="T"/>.</returns>
    Task<T> GetAsync<T>(
        PrimaryKey<T> key,
        bool? consistentRead = false,
        CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Attempts to retrieve a single item by primary key, returns null if the item does not exist.
    /// </summary>
    /// <typeparam name="T">The item type stored in the table.</typeparam>
    /// <param name="key">The primary key of the item to retrieve.</param>
    /// <param name="consistentRead">If <c>true</c>, performs a strongly consistent read. If <c>null</c>, uses the table default.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>The retrieved item of type <typeparamref name="T"/>, or <c>null</c> if not found.</returns>
    Task<T?> TryGetAsync<T>(
        PrimaryKey<T> key,
        bool? consistentRead = false,
        CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Puts (inserts or replaces) an item into the table.
    /// </summary>
    /// <typeparam name="T">The item type stored in the table.</typeparam>
    /// <param name="item">The item to be stored in the table.</param>
    /// <param name="condition">An optional check condition expression evaluated against the item before it is stored.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>The stored item.</returns>
    Task<T> PutAsync<T>(
        T item,
        Expression<Func<T, bool>>? condition = null,
        CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Puts (inserts or replaces) an item into the table.
    /// Convenience overload of <see cref="PutAsync{T}(T, Expression{Func{T, bool}}?, CancellationToken)"/>
    /// that accepts a parameterless condition and rewrites it to reference the provided item.
    /// </summary>
    /// <typeparam name="T">The item type stored in the table.</typeparam>
    /// <param name="item">The item to be stored in the table.</param>
    /// <param name="condition">An optional check condition expression evaluated against the item before it is stored.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>The stored item.</returns>
    Task<T> PutAsync<T>(
        T item,
        Expression<Func<bool>> condition,
        CancellationToken cancellationToken = default) where T : class =>
        PutAsync(
            item,
            condition?.ReplaceConstantWithParameter(item),
            cancellationToken);

    /// <summary>
    /// Updates an item by primary key (or creates a new item if no condition is specified) using the provided update actions.
    /// </summary>
    /// <typeparam name="T">The item type stored in the table.</typeparam>
    /// <param name="key">The primary key of the item to update.</param>
    /// <param name="update">An expression describing update actions to apply to the item.</param>
    /// <param name="condition">An optional check condition expression evaluated against the item before it is updated.</param>
    /// <param name="version">Optional version value used to enforce optimistic concurrency.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>The updated item.</returns>
    Task<T> UpdateAsync<T>(
        PrimaryKey<T> key,
        Expression<Func<T, DynamoDBExpressions.UpdateAction>> update,
        Expression<Func<T, bool>>? condition = null,
        object? version = null,
        CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Updates an item by primary key (or creates a new item if no condition is specified) using the provided update actions.
    /// Convenience overload of <see cref="UpdateAsync{T}(PrimaryKey{T}, Expression{Func{T, DynamoDBExpressions.UpdateAction}}, Expression{Func{T, bool}}?, object?, CancellationToken)"/>
    /// that accepts an item instance and parameterless update/condition expressions which are rewritten to reference the provided item.
    /// </summary>
    /// <typeparam name="T">The item type stored in the table.</typeparam>
    /// <param name="item">The item to update.</param>
    /// <param name="update">An expression describing update actions to apply to the item.</param>
    /// <param name="condition">An optional check condition expression evaluated against the item before it is updated.</param>
    /// <param name="version">Optional version value used to enforce optimistic concurrency.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>The updated item.</returns>
    Task<T> UpdateAsync<T>(
        T item,
        Expression<Func<DynamoDBExpressions.UpdateAction>> update,
        Expression<Func<bool>>? condition = null,
        object? version = null,
        CancellationToken cancellationToken = default) where T : class =>
        UpdateAsync(
            PrimaryKey.ForItem(item),
            update.ReplaceConstantWithParameter(item),
            condition?.ReplaceConstantWithParameter(item),
            version,
            cancellationToken);

    /// <summary>
    /// Deletes an item by primary key.
    /// </summary>
    /// <typeparam name="T">The item type stored in the table.</typeparam>
    /// <param name="key">The primary key of the item to delete.</param>
    /// <param name="condition">An optional check condition expression evaluated against the item before it is deleted.</param>
    /// <param name="version">Optional version value used to enforce optimistic concurrency.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    Task DeleteAsync<T>(
        PrimaryKey<T> key,
        Expression<Func<T, bool>>? condition = null,
        object? version = null,
        CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Deletes an item by primary key.
    /// Convenience overload of <see cref="DeleteAsync{T}(PrimaryKey{T}, Expression{Func{T, bool}}?, object?, CancellationToken)"/>
    /// that accepts an item instance and rewrites the optional condition expression to reference that item if needed.
    /// </summary>
    /// <typeparam name="T">The item type stored in the table.</typeparam>
    /// <param name="item">The item to delete.</param>
    /// <param name="condition">An optional check condition expression evaluated against the item before it is deleted.</param>
    /// <param name="version">Optional version value used to enforce optimistic concurrency.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    Task DeleteAsync<T>(
        T item,
        Expression<Func<bool>>? condition = null,
        object? version = null,
        CancellationToken cancellationToken = default) where T : class =>
        DeleteAsync(
            PrimaryKey.ForItem(item),
            condition?.ReplaceConstantWithParameter(item),
            version,
            cancellationToken);

    /// <summary>
    /// Performs a table scan across all item with optional filter expression and returns a partial, paginated result set of items.
    /// </summary>
    /// <typeparam name="T">The item type stored in the table.</typeparam>
    /// <param name="filter">Optional filter expression applied to scanned items.</param>
    /// <param name="exclusiveStartKey">Optional primary key to start scanning from (for pagination).</param>
    /// <param name="limit">Optional maximum number of items to return in this page.</param>
    /// <param name="consistentRead">If <c>true</c>, performs a strongly consistent read. If <c>null</c>, uses the table default.</param>
    /// <param name="indexProperties">Optional index to scan instead of the base table.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A partial, paginated result set of items.</returns>
    Task<IDynamoDBPartialResult<T>> ScanAsync<T>(
        Expression<Func<T, bool>>? filter = null,
        PrimaryKey<T> exclusiveStartKey = default,
        int? limit = null,
        bool? consistentRead = false,
        (string?, string?) indexProperties = default,
        CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Queries the table using a key condition and optional filter, returns a partial, paginated result set of items.
    /// </summary>
    /// <typeparam name="T">The item type stored in the table.</typeparam>
    /// <param name="keyCondition">A key condition expression that restricts the query to a partition key (and optionally sort key range).</param>
    /// <param name="filter">Optional filter expression applied in addition to the key condition.</param>
    /// <param name="exclusiveStartKey">Optional primary key to start the query from (for pagination).</param>
    /// <param name="scanIndexForward">If <c>true</c>, results are returned in ascending sort key order; if <c>false</c>, descending.</param>
    /// <param name="limit">Optional maximum number of items to return in this page.</param>
    /// <param name="consistentRead">If <c>true</c>, performs a strongly consistent read. If <c>null</c>, uses the table default.</param>
    /// <param name="indexProperties">Optional explicit index to query instead of automatically inferring it from the key condition expression.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A partial, paginated result set of items matching the query.</returns>
    Task<IDynamoDBPartialResult<T>> QueryAsync<T>(
        Expression<Func<T, bool>> keyCondition,
        Expression<Func<T, bool>>? filter = null,
        PrimaryKey<T> exclusiveStartKey = default,
        bool? scanIndexForward = null,
        int? limit = null,
        bool? consistentRead = false,
        (string?, string?) indexProperties = default,
        CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Begins a transaction that batches multiple write operations (put/update/delete) and check conditions into a single operation.
    /// </summary>
    /// <returns>An <see cref="IDynamoDBWriteTransaction"/> instance.</returns>
    IDynamoDBWriteTransaction BeginWriteTransaction();
}
