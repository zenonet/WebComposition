namespace Core;

public class FunctionDefinition
{
    public string? Name;
    public string[] ParameterNames;
    public bool IsComposable = false;
    public List<Executable> Executables;

    public void ExecuteWithoutParameters()
    {
        foreach (Executable e in Executables)
        {
            e.Execute();
        }
    }
}