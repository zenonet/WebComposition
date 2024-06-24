namespace Generator;

public class VariableGetter : Executable
{
    public string VariableName;

    public VariableGetter(string variableName)
    {
        VariableName = variableName;
        Dependencies = [VariableName];
    }

    public override Value Execute()
    {
        if (!VariableSetter.VariableValues.TryGetValue(VariableName, out Variable? var))
            throw new($"Variable {VariableName} has never been declared");
        return var.Value;
    }
}