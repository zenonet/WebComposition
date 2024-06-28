namespace Core;

public class IfStatement : Executable
{
    public required Executable Condition;
    public required List<Executable> Block;
    public List<Executable>? ElseBlock;
    public override Value Execute()
    {
        if (Condition.Execute() is not BoolValue condition) throw new LanguageException("If statement only accepts boolean values as conditions", LineNumber);
        if (condition.Value)
        {
            foreach (Executable e in Block)
            {
                e.Execute();
            }
        }
        else if(ElseBlock != null)
        {
            foreach (Executable e in ElseBlock)
            {
                e.Execute();
            }
        }
        return VoidValue.I;
    }
}