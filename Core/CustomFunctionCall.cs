namespace Core;

public class CustomFunctionCall : Function
{
    public required FunctionDefinition Definition;

    public override Value Execute()
    {
        if (Definition.ParameterNames.Length != Parameters.Count)
            throw new LanguageException($"Function {Definition.Name} expects {Definition.ParameterNames.Length} parameters but received {Parameters.Count}", LineNumber);

        // Evaluate all parameters and add their variables
        for (int i = 0; i < Definition.ParameterNames.Length; i++)
        {
            VariableSetter.VariableValues[Definition.ParameterNames[i]] = new() {Value = Parameters[i].Execute()};
        }
        // TODO: Make sure to remove these variables after the function execution is completed

        foreach (Executable e in Definition.Executables)
        {
            e.Execute();
        }

        return VoidValue.I;
    }
}