namespace Core;

public class VariableSetter : Executable
{
    public static Dictionary<string, Variable> VariableValues = new();
    
    public string VariableName;
    public Executable Value;

    /// <summary>
    /// Whether this setter is supposed to only run once
    /// </summary>
    private bool didInit;
    public bool IsInitOnly;
    public override Value Execute()
    {
        if(IsInitOnly && didInit) return VoidValue.I;
        
        VariableValues[VariableName].Value = Value.Execute();
        VariableValues[VariableName].OnVariableChanged();
        didInit = true;
        return VoidValue.I;
    }
}