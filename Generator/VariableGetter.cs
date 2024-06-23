namespace Generator;

public class VariableGetter : Executable
{
    public string VariableName;

    public override Value Execute()
    {
        if (!VariableSetter.VariableValues.TryGetValue(VariableName, out Value? val))
            throw new($"Variable {VariableName} has never been declared");
        return val;
    }
}