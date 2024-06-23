namespace Generator;

public class VariableSetter : Executable
{
    public static Dictionary<string, Value> VariableValues = new();
    
    public string VariableName;
    public Executable Value;
    public override Value Execute()
    {
        VariableValues[VariableName] = Value.Execute();
        return VoidValue.I;
    }
}