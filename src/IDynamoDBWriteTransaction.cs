using System.Linq.Expressions;
using DynamoDB.Net.Expressions;

namespace DynamoDB.Net;

/// <summary>
/// Represents a transaction that batches multiple write operations (put/update/delete) and check conditions into a single operation.
/// </summary>
public interface IDynamoDBWriteTransaction
{
    /// <summary>
    /// Puts (inserts or replaces) an item into the table.
    /// Convenience overload of <see cref="Put{T}(T, Expression{Func{T, bool}}?)"/>
    /// that accepts a parameterless condition and rewrites it to reference the provided item.
    /// </summary>
    /// <typeparam name="T">The item type stored in the table.</typeparam>
    /// <param name="item">The item to be stored in the table.</param>
    /// <param name="condition">An optional check condition expression evaluated against the item before it is stored.</param>
    /// <returns>The same <see cref="IDynamoDBWriteTransaction"/> instance for method call chaining convenience.</returns>
    IDynamoDBWriteTransaction Put<T>(
        T item,
        Expression<Func<T, bool>>? condition = null) where T : class;

    /// <summary>
    /// Puts (inserts or replaces) an item into the table.
    /// Convenience overload of <see cref="Put{T}(T, Expression{Func{T, bool}}?)"/>
    /// that accepts a parameterless condition and rewrites it to reference the provided item.
    /// </summary>
    /// <typeparam name="T">The item type stored in the table.</typeparam>
    /// <param name="item">The item to be stored in the table.</param>
    /// <param name="condition">An optional check condition expression evaluated against the item before it is stored.</param>
    /// <returns>The same <see cref="IDynamoDBWriteTransaction"/> instance for method call chaining convenience.</returns>
    IDynamoDBWriteTransaction Put<T>(
        T item,
        Expression<Func<bool>> condition) where T : class =>
        Put(
            item,
            condition?.ReplaceConstantWithParameter(item));

    /// <summary>
    /// Updates an item by primary key (or creates a new item if no condition is specified) using the provided update actions.
    /// </summary>
    /// <typeparam name="T">The item type stored in the table.</typeparam>
    /// <param name="key">The primary key of the item to update.</param>
    /// <param name="update">An expression describing update actions to apply to the item.</param>
    /// <param name="condition">An optional check condition expression evaluated against the item before it is updated.</param>
    /// <param name="version">Optional version value used to enforce optimistic concurrency.</param>
    /// <returns>The same <see cref="IDynamoDBWriteTransaction"/> instance for method call chaining convenience.</returns>
    IDynamoDBWriteTransaction Update<T>(
        PrimaryKey<T> key,
        Expression<Func<T, DynamoDBExpressions.UpdateAction>> update,
        Expression<Func<T, bool>>? condition = null,
        object? version = null) where T : class;

    /// <summary>
    /// Updates an item by primary key (or creates a new item if no condition is specified) using the provided update actions.
    /// Convenience overload of <see cref="Update{T}(PrimaryKey{T}, Expression{Func{T, DynamoDBExpressions.UpdateAction}}, Expression{Func{T, bool}}?, object?)"/>
    /// that accepts an item instance and parameterless update/condition expressions which are rewritten to reference the provided item.
    /// </summary>
    /// <typeparam name="T">The item type stored in the table.</typeparam>
    /// <param name="item">The item to update.</param>
    /// <param name="update">An expression describing update actions to apply to the item.</param>
    /// <param name="condition">An optional check condition expression evaluated against the item before it is updated.</param>
    /// <param name="version">Optional version value used to enforce optimistic concurrency.</param>
    /// <returns>The same <see cref="IDynamoDBWriteTransaction"/> instance for method call chaining convenience.</returns>
    IDynamoDBWriteTransaction Update<T>(
        T item,
        Expression<Func<DynamoDBExpressions.UpdateAction>> update,
        Expression<Func<bool>>? condition = null,
        object? version = null) where T : class =>
        Update(
            PrimaryKey.ForItem(item),
            update.ReplaceConstantWithParameter(item),
            condition?.ReplaceConstantWithParameter(item),
            version);

    /// <summary>
    /// Deletes an item by primary key.
    /// </summary>
    /// <typeparam name="T">The item type stored in the table.</typeparam>
    /// <param name="key">The primary key of the item to delete.</param>
    /// <param name="condition">An optional check condition expression evaluated against the item before it is deleted.</param>
    /// <param name="version">Optional version value used to enforce optimistic concurrency.</param>
    /// <returns>The same <see cref="IDynamoDBWriteTransaction"/> instance for method call chaining convenience.</returns>
    IDynamoDBWriteTransaction Delete<T>(
        PrimaryKey<T> key,
        Expression<Func<T, bool>>? condition = null,
        object? version = null) where T : class;

    /// <summary>
    /// Deletes an item by primary key.
    /// Convenience overload of <see cref="Delete{T}(PrimaryKey{T}, Expression{Func{T, bool}}?, object?)"/>
    /// that accepts an item instance and rewrites the optional condition expression to reference that item if needed.
    /// </summary>
    /// <typeparam name="T">The item type stored in the table.</typeparam>
    /// <param name="item">The item to delete.</param>
    /// <param name="condition">An optional check condition expression evaluated against the item before it is deleted.</param>
    /// <param name="version">Optional version value used to enforce optimistic concurrency.</param>
    /// <returns>The same <see cref="IDynamoDBWriteTransaction"/> instance for method call chaining convenience.</returns>
    IDynamoDBWriteTransaction Delete<T>(
        T item,
        Expression<Func<bool>>? condition = null,
        object? version = null) where T : class =>
        Delete(
            PrimaryKey.ForItem(item),
            condition?.ReplaceConstantWithParameter(item),
            version);

    /// <summary>
    /// Perform a condition check on an item without modifiying it.
    /// </summary>
    /// <typeparam name="T">The item type stored in the table.</typeparam>
    /// <param name="key">The primary key of the item to perform a condition check on.</param>
    /// <param name="condition">An check condition expression evaluated against the item for the transaction to be valid.</param>
    /// <param name="version">Optional version value used to enforce optimistic concurrency.</param>
    /// <returns>The same <see cref="IDynamoDBWriteTransaction"/> instance for method call chaining convenience.</returns>
    IDynamoDBWriteTransaction ConditionCheck<T>(
        PrimaryKey<T> key,
        Expression<Func<T, bool>> condition,
        object? version = null) where T : class;

    /// <summary>
    /// Perform a condition check on an item without modifiying it.
    /// Convenience overload of <see cref="ConditionCheck{T}(PrimaryKey{T}, Expression{Func{T, bool}}?, object?)"/>
    /// that accepts an item instance and rewrites the optional condition expression to reference that item if needed.
    /// </summary>
    /// <typeparam name="T">The item type stored in the table.</typeparam>
    /// <param name="item">The item to perform a condition check on.</param>
    /// <param name="condition">An check condition expression evaluated against the item for the transaction to be valid.</param>
    /// <param name="version">Optional version value used to enforce optimistic concurrency.</param>
    /// <returns>The same <see cref="IDynamoDBWriteTransaction"/> instance for method call chaining convenience.</returns>
    IDynamoDBWriteTransaction ConditionCheck<T>(
        T item,
        Expression<Func<bool>> condition,
        object? version = null) where T : class =>
        ConditionCheck(
            PrimaryKey.ForItem(item),
            condition.ReplaceConstantWithParameter(item),
            version);

    /// <summary>
    /// Commits the transaction.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    Task CommitAsync(CancellationToken cancellationToken = default);
}
