namespace DynamoDB.Net;

/// <summary>
/// Represents a partial, paginated result returned from a scan or query operation.
/// </summary>
public interface IDynamoDBPartialResult : IDynamoDBPartialResult<object, IPrimaryKey>
{
}

/// <summary>
/// Represents a partial, paginated result returned from a scan or query operation.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public interface IDynamoDBPartialResult<T> : IDynamoDBPartialResult<T, PrimaryKey<T>> 
    where T : class
{
}

/// <summary>
/// Represents a partial, paginated result returned from a scan or query operation.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
/// <typeparam name="TKey">The primary key type for.</typeparam>
public interface IDynamoDBPartialResult<T, out TKey> : IReadOnlyList<T>
    where T : class
    where TKey : IPrimaryKey
{
    /// <summary>
    /// The last evaluated primary key for this page of results. 
    /// Use this value as the exclusive start key when requesting the next page. 
    /// If no further pages exist this value will be the default value for <typeparamref name="TKey"/>.
    /// </summary>
    TKey LastEvaluatedKey { get; }
}
