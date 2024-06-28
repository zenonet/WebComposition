namespace Core;

public class Expression : Executable
{
    public Executable LeftOperand;
    public Executable RightOperand;
    public OperatorType Operator;

    public override Value Execute()
    {
        var l = LeftOperand.Execute();
        var r = RightOperand.Execute();

        ExpressionHelper.LineNumberForErrors = LineNumber;
        Value result = Operator switch
        {
            OperatorType.Plus => new IntValue {Value = l.As<int>() + r.As<int>()},
            OperatorType.Minus => new IntValue {Value = l.As<int>() - r.As<int>()},
            OperatorType.Multiply => new IntValue {Value = l.As<int>() * r.As<int>()},
            OperatorType.Divide => new IntValue {Value = l.As<int>() / r.As<int>()},
            
            
            OperatorType.Equals => new BoolValue {Value = l.As<int>() == r.As<int>()},
            
            OperatorType.GreaterThan => new BoolValue {Value = l.As<int>() > r.As<int>()},
            OperatorType.GreaterThanOrEqualTo => new BoolValue {Value = l.As<int>() >= r.As<int>()},
            
            OperatorType.LessThan => new BoolValue {Value = l.As<int>() < r.As<int>()},
            OperatorType.LessThanOrEqualTo => new BoolValue {Value = l.As<int>() <= r.As<int>()},
        };
        ExpressionHelper.LineNumberForErrors = -1;
        return result;
    }
}

public static class ExpressionHelper
{
    public static int LineNumberForErrors = -1;
    public static T As<T>(this Value v)
    {
        var rightType = Value.ValueTranslationTable[typeof(T)];
        if (!rightType.IsAssignableTo(v.GetType()))
            throw new LanguageException("Invalid types of operands in expression", LineNumberForErrors);
        return (T) v.GetType().GetProperty("Value")!.GetValue(v)! ?? throw new LanguageException($"No value property defined in the language abstraction of the {v.GetType().Name} type.", LineNumberForErrors);
    }
}