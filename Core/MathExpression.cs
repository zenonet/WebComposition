namespace Core;

public class MathExpression : Executable
{
    public Executable LeftOperand;
    public Executable RightOperand;
    public char Operator;

    public override Value Execute()
    {
        var left = LeftOperand.Execute();
        var right = RightOperand.Execute();
        if (left is not IntValue l || right is not IntValue r)
            throw new("Math expressions with values other than ints currently aren't supported");

        var result = Operator switch
        {
            '+' => l.Value + r.Value,
            '-' => l.Value - r.Value,
            '*' => l.Value * r.Value,
            '/' => l.Value / r.Value,
        };
        return new IntValue{Value = result};
    }
}