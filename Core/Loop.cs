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
            if (Condition.Execute() is not BoolValue cond) throw new LanguageException("Condition of loop has to be of boolean type", LineNumber);
            if (!cond.Value) break;
            
            foreach (Executable e in Block)
            {
                e.Execute();
            }

            IncrementStatement?.Execute();
        }
        return VoidValue.I;
    }
}