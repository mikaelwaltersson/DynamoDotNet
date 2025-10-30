namespace DynamoDB.Net.Serialization;

/// <summary>
/// Resolves and encodes type names for polymorphic object storage in DynamoDB.
/// </summary>
public abstract class DynamoDBObjectTypeNameResolver
{
    /// <summary>
    /// Default resolver that uses the assembly qualified type name.
    /// </summary>
    public static readonly DynamoDBObjectTypeNameResolver Default = new DefaultResolver();

    /// <summary>
    /// The attribute name used to store the type identifier on DynamoDB items.
    /// </summary>
    public abstract string Attribute { get; }

    /// <summary>
    /// Resolves the type for the given stored type name.
    /// </summary>
    /// <param name="typeName">The type name to resolve the object type for.</param>
    public abstract Type GetObjectType(string typeName);

    /// <summary>
    /// Returns the string representation used to identify the specified type.
    /// </summary>
    /// <param name="objectType">The object type to resolve the type name for.</param>
    public abstract string GetTypeName(Type objectType);

    class DefaultResolver : DynamoDBObjectTypeNameResolver
    {
        public override string Attribute => "$type";

        public override Type GetObjectType(string typeName) =>
            Type.GetType(typeName, throwOnError: false) ?? throw new DynamoDBSerializationException($"Can not resolve type from '{typeName}'");

        public override string GetTypeName(Type objectType) =>
            objectType.AssemblyQualifiedName ?? throw new DynamoDBSerializationException($"Can not resolve type name for '{objectType}'");
    }
}
