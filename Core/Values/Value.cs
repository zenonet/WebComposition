namespace Core;

public abstract class Value
{
    protected object? O;

    public static readonly Dictionary<Type, Type> ValueTranslationTable = new()
    {
        {typeof(int), typeof(IntValue)},
        {typeof(string), typeof(StringValue)},
        {typeof(bool), typeof(BoolValue)},
    };
}