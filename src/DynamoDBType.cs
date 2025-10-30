namespace DynamoDB.Net;

/// <summary>
/// Represents the different DynamoDB data types.
/// </summary>
public enum DynamoDBType
{
    /// <summary>
    /// The <c>NULL</c> data type.
    /// </summary>
    Null,

    /// <summary>
    /// The <c>BOOL</c> data type.
    /// </summary>
    Bool,

    /// <summary>
    /// The <c>S</c> data type.
    /// </summary>
    String,

    /// <summary>
    /// The <c>N</c> data type.
    /// </summary>
    Number,

    /// <summary>
    /// The <c>B</c> data type.
    /// </summary>
    Binary,

    /// <summary>
    /// The <c>SS</c> data type.
    /// </summary>
    StringSet,

    /// <summary>
    /// The <c>NS</c> data type.
    /// </summary>
    NumberSet,

    /// <summary>
    /// The <c>BS</c> data type.
    /// </summary>
    BinarySet,

    /// <summary>
    /// The <c>L</c> data type.
    /// </summary>
    List,

    /// <summary>
    /// The <c>M</c> data type.
    /// </summary>
    Map
}
