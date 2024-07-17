namespace Core;

public class CustomFunctionCall : Executable
{
    public required FunctionDefinition Definition;
    public List<Executable> Parameters;
    public override Value Execute()
    {
        // TOOD: Create some sort of context for the executables and assign the parameters
        foreach (Executable e in Definition.Executables)
        {
            e.Execute();
        }
        return VoidValue.I;
    }
}