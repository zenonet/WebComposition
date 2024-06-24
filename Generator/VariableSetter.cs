namespace Generator;

public class VariableSetter : Executable
{
    public static Dictionary<string, Variable> VariableValues = new();
    
    public string VariableName;
    public Executable Value;
    public override Value Execute()
    {
        VariableValues[VariableName].Value = Value.Execute();
        VariableValues[VariableName].OnVariableChanged();
        return VoidValue.I;
    }
}