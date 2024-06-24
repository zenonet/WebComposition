namespace Core;

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
        if (var.Value == VoidValue.Uninitialized) throw new($"Variable {VariableName} hasn't been initialized yet.");
        return var.Value;
    }
}