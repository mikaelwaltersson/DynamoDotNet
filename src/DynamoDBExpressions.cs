using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace DynamoDB.Net;

/// <summary>
/// Provides method signatures that represent DynamoDB operators
/// for use in item condition and update expressions.
/// </summary>
[ExcludeFromCodeCoverage]
public static class DynamoDBExpressions
{
    /// <summary>
    /// Translates to the DynamoDB '=' operator.
    /// </summary>
    [TranslatesTo("{0} = {1}")]
    public static bool EqualTo<T>(T operand1, T operand2) => DynamoDBMethod<bool>();

    /// <summary>
    /// Translates to the DynamoDB '&lt;&gt;' operator.
    /// </summary>
    [TranslatesTo("{0} <> {1}")]
    public static bool NotEqualTo<T>(T operand1, T operand2) => DynamoDBMethod<bool>();

    /// <summary>
    /// Translates to the DynamoDB '&lt;' operator.
    /// </summary>
    [TranslatesTo("{0} < {1}")]
    public static bool LessThan<T>(T operand1, T operand2) => DynamoDBMethod<bool>();
    
    /// <summary>
    /// Translates to the DynamoDB '&lt;=' operator.
    /// </summary>
    [TranslatesTo("{0} <= {1}")]
    public static bool LessThanOrEqualTo<T>(T operand1, T operand2) => DynamoDBMethod<bool>();

    /// <summary>
    /// Translates to the DynamoDB '&gt;' DynamoDB .
    /// </summary>
    [TranslatesTo("{0} > {1}")]
    public static bool GreaterThan<T>(T operand1, T operand2) => DynamoDBMethod<bool>();

    /// <summary>
    /// Translates to the DynamoDB '&gt;=' operator.
    /// </summary>
    [TranslatesTo("{0} >= {1}")]
    public static bool GreaterThanOrEqualTo<T>(T operand1, T operand2) => DynamoDBMethod<bool>();

    /// <summary>
    /// Translates to the DynamoDB 'BETWEEN' operator.
    /// </summary>
    [TranslatesTo("{0} BETWEEN {1} AND {2}")]
    public static bool Between<T>(T operand1, T operand2, T operand3) => DynamoDBMethod<bool>();

    /// <summary>
    /// Translates to the DynamoDB 'IN (...)' operator.
    /// </summary>
    [TranslatesTo("{0} IN ({1})", hasParams: true)]
    public static bool In<T>(T operand1, params T[] operands) => DynamoDBMethod<bool>();

    /// <summary>
    /// Translates to the DynamoDB 'attribute_exists(path)' function.
    /// </summary>
    [TranslatesTo("attribute_exists({0})")]
    public static bool AttributeExists<T>(T path) => DynamoDBMethod<bool>();
    
    /// <summary>
    /// Translates to the DynamoDB 'attribute_not_exists(path)' function.
    /// </summary>
    [TranslatesTo("attribute_not_exists({0})")]
    public static bool AttributeNotExists<T>(T path) => DynamoDBMethod<bool>();

    /// <summary>
    /// Translates to the DynamoDB 'attribute_type(path, type)' function.
    /// </summary>
    [TranslatesTo("attribute_type({0}, {1})")]
    public static bool AttributeType<T>(T path, DynamoDBType type) => DynamoDBMethod<bool>();

    /// <summary>
    /// Translates to the DynamoDB 'begins_with(path, substr)' function.
    /// </summary>
    [TranslatesTo("begins_with({0}, {1})")]
    public static bool BeginsWith<T>(T path, T substr) => DynamoDBMethod<bool>();
    
    /// <summary>
    /// Translates to the DynamoDB 'contains(path, operand)' function.
    /// </summary>
    [TranslatesTo("contains({0}, {1})")]
    public static bool Contains<T>(T path, T operand) => DynamoDBMethod<bool>();

    /// <summary>
    /// Translates to the DynamoDB 'contains(path, operand)' function.
    /// </summary>
    [TranslatesTo("contains({0}, {1})")]
    public static bool Contains<T>(IEnumerable<T> path, T operand) => DynamoDBMethod<bool>();

    /// <summary>
    /// Translates to the DynamoDB 'size(path)' function.
    /// </summary>
    [TranslatesTo("size({0})")]
    public static int Size<T>(T path) => DynamoDBMethod<int>();

    /// <summary>
    /// Translates to the DynamoDB 'if_not_exists(path, operand)' function.
    /// </summary>
    [TranslatesTo("if_not_exists({0}, {1})")]
    public static T IfNotExists<T>(T path, T operand) => DynamoDBMethod<T>();

    /// <summary>
    /// Translates to the DynamoDB 'list_append(list1, list2) function'.
    /// </summary>
    [TranslatesTo("list_append({0}, {1})", arrayConstantKind: ArrayConstantKind.List)]
    public static IEnumerable<T> ListAppend<T>(IEnumerable<T>? list1, IEnumerable<T>? list2) => DynamoDBMethod<IEnumerable<T>>();

    /// <summary>
    /// Translates to the DynamoDB 'ADD' update operation.
    /// </summary>
    [TranslatesTo("ADD {0} {1}")]
    public static UpdateAction Add<T>(T path, T value) => DynamoDBMethod<UpdateAction>();

    /// <summary>
    /// Translates to the DynamoDB 'ADD' update operation.
    /// </summary>
    [TranslatesTo("ADD {0} {1}", arrayConstantKind: ArrayConstantKind.Set)]
    public static UpdateAction Add<T>(IEnumerable<T>? path, IEnumerable<T>? value) => DynamoDBMethod<UpdateAction>();

    /// <summary>
    /// Translates to the DynamoDB 'SET' update operation.
    /// </summary>
    [TranslatesTo("SET {0} = {1}", arrayConstantKind: ArrayConstantKind.FromFirstOperandType)]
    public static UpdateAction Set<T>(T path, T value) => DynamoDBMethod<UpdateAction>();

    /// <summary>
    /// Translates to the DynamoDB 'REMOVE' update operation.
    /// </summary>
    [TranslatesTo("REMOVE {0}")]
    public static UpdateAction Remove<T>(T path) => DynamoDBMethod<UpdateAction>();

    /// <summary>
    /// Translates to the DynamoDB 'REMOVE' update operation.
    /// </summary>
    [TranslatesTo("REMOVE {0}", hasParams: true)]
    public static UpdateAction Remove(params object[] paths) => DynamoDBMethod<UpdateAction>();

    /// <summary>
    /// Translates to the DynamoDB 'DELETE' update operation.
    /// </summary>
    [TranslatesTo("DELETE {0} {1}", arrayConstantKind: ArrayConstantKind.Set)]
    public static UpdateAction Delete<T>(IEnumerable<T>? path, IEnumerable<T>? value) => DynamoDBMethod<UpdateAction>();

    /// <summary>
    /// Represents an empty DynamoDB update operation, can be used when dynamically creating
    /// update operations, will be ignored during translation.
    /// </summary>
    [TranslatesTo("")]
    public static UpdateAction NoOp<T>(T path) => DynamoDBMethod<UpdateAction>();

    /// <summary>
    /// Represents a constant value in expressions, used to ensure that a value is
    /// resolved to a constant and not for example a item path reference.
    /// </summary>
    [TranslatesTo("")]
    public static T Constant<T>(T value) => value;

    /// <summary>
    /// Base type for providing a raw expression string together with any placeholder
    /// name and value aliases that should be merged into the translation context.
    /// </summary>
    public abstract class RawExpression
    {
        internal readonly string expression;
        internal Dictionary<string, string>? names;
        internal Dictionary<string, object>? values;

        /// <summary>
        /// Initializes a new instance of <see cref="RawExpression"/> with the provided expression text.
        /// </summary>
        public RawExpression(string expression)
        {
            ArgumentNullException.ThrowIfNull(expression);

            this.expression = expression;
        }

        /// <summary>
        /// Gets the dictionary that maps attribute name placeholders to actual attribute names.
        /// </summary>
        public Dictionary<string, string> Names => names ??= [];

        /// <summary>
        /// Gets the dictionary that maps attribute value placeholders to their CLR values.
        /// </summary>
        public Dictionary<string, object> Values => values ??= [];
    }

    /// <summary>
    /// Used for providing a raw expression string together with any placeholder
    /// name and value aliases that should be merged into the translation context.
    /// </summary>
    public sealed class RawExpression<T>(string expression) : RawExpression(expression) where T : class
    {
        /// <summary>
        /// Cast the raw expression into a predicate expression delegate for the target type.
        /// </summary>
        public static implicit operator Expression<Func<T, bool>>(RawExpression<T> expression) => expression.ToExpression<bool>();

        /// <summary>
        /// Cast the raw expression into an update action expression delegate for the target type.
        /// </summary>
        public static implicit operator Expression<Func<T, UpdateAction>>(RawExpression<T> expression) => expression.ToExpression<UpdateAction>();

        Expression<Func<T, TResult>> ToExpression<TResult>() => _ => (TResult)(object)this;
    }

    [AttributeUsage(AttributeTargets.Method)]
    internal class TranslatesTo(string format, bool hasParams = false, ArrayConstantKind arrayConstantKind = default) : Attribute
    {
        public string Format { get; } = format;

        public bool HasParams { get; } = hasParams;

        public ArrayConstantKind ArrayConstantKind { get; } = arrayConstantKind;
    }

    internal enum ArrayConstantKind
    {
        Unspecified,

        List,

        Set,

        FromFirstOperandType
    }

    /// <summary>
    /// Placeholder type used to represent a composed update action during expression translation.
    /// </summary>
    public struct UpdateAction
    {
        /// <summary>
        /// Combines two update actions into a single action using the '&amp;' operator.
        /// </summary>
        public static UpdateAction operator &(UpdateAction operand1, UpdateAction operand2) => DynamoDBMethod<UpdateAction>();
    }

    static T DynamoDBMethod<T>([CallerMemberName] string? caller = null) => 
        throw new InvalidOperationException($"Invalid usage of DynamoDBExpressions.{caller}(...), are all argument non-parameter values?");
}
