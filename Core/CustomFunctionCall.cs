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
            // Ensure there's no ambiguity between variable names and parameter names
            if (VariableSetter.VariableValues.ContainsKey(Definition.ParameterNames[i]))
                throw new LanguageException($"Invalid parameter name {Definition.ParameterNames[i]} for function '{Definition.Name}()'. There is already a variable with the same name declared.", LineNumber);

            // Temporarily add parameters as global variables
            VariableSetter.VariableValues[Definition.ParameterNames[i]] = new() {Value = Parameters[i].Execute()};
        }

        foreach (Executable e in Definition.Executables)
            e.Execute();

        // Remove parameters from variables
        foreach (string t in Definition.ParameterNames)
            VariableSetter.VariableValues.Remove(t);

        return VoidValue.I;
    }
}