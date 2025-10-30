namespace DynamoDB.Net;

/// <summary>
/// Exception thrown when an item cannot be found in a DynamoDB table.
/// </summary>
public abstract class ItemNotFoundException(string message) : Exception(message)
{
    /// <summary>
    /// The name of the table in which the item was not found.
    /// </summary>
    public abstract string TableName { get; }

    /// <summary>
    /// The primary key that was used in the attempt to retreive the item.
    /// </summary>
    public abstract IPrimaryKey Key { get; }
}

/// <inheritdoc />
/// <typeparam name="T">The item type stored in the table.</typeparam>
public class ItemNotFoundException<T>(PrimaryKey<T> key, string tableName)
    : ItemNotFoundException($"Item with key {key} not found in table {tableName}") where T : class
{
    /// <inheritdoc />
    public override string TableName => tableName;

    /// <inheritdoc />
    public override IPrimaryKey Key => key;
}
