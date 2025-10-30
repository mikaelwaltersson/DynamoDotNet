namespace DynamoDB.Net.Serialization;

/// <summary>
/// Name transformer for transforming property or enum names into the serialized version.
/// </summary>
public abstract class NameTransform
{
    /// <summary>
    /// Default name transformer, no transformation of the name.
    /// </summary>
    public static readonly NameTransform Default = new Transform(name => name);

    /// <summary>
    /// Name transformer for camel case.
    /// </summary>
    public static readonly NameTransform CamelCase = new Transform(NameTransformStringExtensions.ToCamelCase);

    /// <summary>
    /// Name transformer for snake case.
    /// </summary>
    public static readonly NameTransform SnakeCase = new Transform(NameTransformStringExtensions.ToSnakeCase);

    /// <summary>
    /// Name transformer for hyphen case.
    /// </summary>
    public static readonly NameTransform HyphenCase = new Transform(NameTransformStringExtensions.ToHyphenCase);

    /// <summary>
    /// Transform a name into it's serialized representation.
    /// </summary>
    public abstract string TransformName(string name);

    class Transform(Func<string, string> transform) : NameTransform
    {
        public override string TransformName(string name) => transform(name);
    }
}
