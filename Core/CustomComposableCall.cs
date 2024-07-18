namespace Core;

public class CustomComposableCall : Composable
{
    public required FunctionDefinition Definition;
    
    // Yeah, I know, this should not be there and instead BlockComposable.Block should be used but that would mean, I'd need a third subclass for custom*call
    // I just should use inheritance here and solve this with composition, but now it's too late and I (the author) probably am you (the only reader) anyway
    public FunctionDefinition? ContentBlock;
    public override string GenerateHtml()
    {
        if (Definition.ParameterNames.Length != Parameters.Count)
            throw new LanguageException($"Composable {Definition.Name} expects {Definition.ParameterNames.Length} parameters but received {Parameters.Count}", LineNumber);

        // Evaluate all parameters and add their variables
        for (int i = 0; i < Definition.ParameterNames.Length; i++)
        {
            // Ensure there's no ambiguity between variable names and parameter names
            if (VariableSetter.VariableValues.ContainsKey(Definition.ParameterNames[i]))
                throw new LanguageException($"Invalid parameter name {Definition.ParameterNames[i]} for composable '{Definition.Name}()'. There is already a variable with the same name declared.", LineNumber);

            // Temporarily add parameters as global variables
            VariableSetter.VariableValues[Definition.ParameterNames[i]] = new() {Value = Parameters[i].Execute()};
        }

        // Add the content composable to the context
        if (ContentBlock != null) ContentComposableCall.ContentFunctionStack.Push(ContentBlock);

        string html = ExecuteAndGetHtml(Definition.Executables);

        // Remove the content composable from the context
        if (ContentBlock != null)ContentComposableCall.ContentFunctionStack.Pop();
        
        // Remove parameters from variables
        foreach (string t in Definition.ParameterNames)
            VariableSetter.VariableValues.Remove(t);

        return html;
    }
}