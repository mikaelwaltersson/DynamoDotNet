namespace DynamoDB.Net.Serialization;

/// <summary>
/// Optional handler interface invoked after a property value has been deserialized.
/// Implementers can transform or validate the deserialized value and assign it to the target object.
/// </summary>
public interface IOnDeserializeProperty
{
    /// <summary>
    /// Called when a property is deserialized for the given <paramref name="target"/>.
    /// Implementers should call <paramref name="setValue"/> to assign the (possibly transformed) value.
    /// </summary>
    /// <typeparam name="T">Type of the target object.</typeparam>
    /// <param name="target">The object instance being deserialized.</param>
    /// <param name="propertyName">Name of the property being deserialized.</param>
    /// <param name="value">The deserialized value.</param>
    /// <param name="setValue">Callback to set the property's value on <paramref name="target"/>.</param>
    void OnDeserialize<T>(T target, string propertyName, object? value, Action<T, object?> setValue) where T : notnull;
}
