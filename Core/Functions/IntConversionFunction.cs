namespace Core.Functions;

public class IntConversionFunction : Function
{
    public override Value Execute()
    {
        EnsureParameterCount(1);
        
        Value val = Parameters[0].Execute();
        if (val is IntValue i)
            return i;
        if (val is StringValue s)
            return new IntValue{Value = int.Parse(s.Value)};
        if (val is BoolValue b)
            return new IntValue {Value = b.Value ? 1 : 0};
        
        throw new LanguageException($"Unable to convert {val.TypeName} to int", LineNumber);
    }
}