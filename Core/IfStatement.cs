namespace Core;

public class IfStatement : Executable
{
    public required Executable Condition;
    public required List<Executable> Block;
    public override Value Execute()
    {
        if (Condition.Execute() is not BoolValue condition) throw new("If statement only accepts boolean values as conditions");
        if (condition.Value)
        {
            foreach (Executable e in Block)
            {
                e.Execute();
            }
        }
        return VoidValue.I;
    }
}