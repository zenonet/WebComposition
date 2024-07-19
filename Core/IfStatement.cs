namespace Core;

public class IfStatement : Executable
{
    public required Executable Condition;
    public required List<Executable> Block;
    public List<Executable>? ElseBlock;
    public override Value Execute()
    {
        BoolValue shouldExecute = Condition.Execute().AsBool(LineNumber);
        if (shouldExecute.Value)
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