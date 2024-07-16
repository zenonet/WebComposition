using System.Diagnostics.CodeAnalysis;

namespace Core;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)]
public abstract class Value
{
    protected object? O;
    public abstract string TypeName { get; }

    public static readonly Dictionary<Type, Type> ValueTranslationTable = new()
    {
        {typeof(int), typeof(IntValue)},
        {typeof(string), typeof(StringValue)},
        {typeof(bool), typeof(BoolValue)},
    };
}