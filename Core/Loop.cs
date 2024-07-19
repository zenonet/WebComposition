namespace Core;

public class Loop : Executable
{
    public required Executable Condition;
    public Executable? IncrementStatement;
    public Executable? InitStatement;
    
    public required List<Executable> Block;

    public override Value Execute()
    {
        InitStatement?.Execute();

        while (true)
        {
            BoolValue shouldContinue = Condition.Execute().AsBool(LineNumber);
            if (!shouldContinue.Value) break;
            
            foreach (Executable e in Block)
            {
                e.Execute();
            }

            IncrementStatement?.Execute();
        }
        return VoidValue.I;
    }
}