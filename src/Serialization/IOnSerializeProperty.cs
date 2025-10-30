namespace DynamoDB.Net.Serialization;

/// <summary>
/// Optional handler interface invoked when a property value is being serialized.
/// Implementers can provide custom serialization logic or value transformation.
/// </summary>
public interface IOnSerializeProperty
{
    /// <summary>
    /// Called to obtain the value to serialize for the specified property on <paramref name="target"/>.
    /// </summary>
    /// <typeparam name="T">Type of the target object.</typeparam>
    /// <param name="target">The object instance containing the property.</param>
    /// <param name="propertyName">Name of the property being serialized.</param>
    /// <param name="getValue">Delegate to read the current property value from <paramref name="target"/>.</param>
    /// <returns>The value to be serialized.</returns>
    object? OnSerialize<T>(T target, string propertyName, Func<T, object?> getValue) where T : notnull;
}
