namespace Core;

public class TernaryConditionalOperator : Executable
{
    public required Executable Condition;
    public required Executable PositiveValue;
    public required Executable NegativeValue;
    public override Value Execute()
    {
        if (Condition.Execute() is not BoolValue condition) throw new("Ternary conditional operator only accepts boolean values as conditions");
        return condition.Value ? PositiveValue.Execute() : NegativeValue.Execute();
    }
}