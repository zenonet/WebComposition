namespace Core;

public class VariableGetter : Executable
{
    public string VariableName;
    public bool IsIncrementOperation;
    public sbyte IncrementDirection;

    public VariableGetter(string variableName)
    {
        VariableName = variableName;
        Dependencies = [VariableName];
    }

    public override Value Execute()
    {
        if (VariableName == "it")
        {
            if (Lambda.CurrentLambdaArgument == null)
                throw new LanguageException("'it' is not defined in the current context", LineNumber);
            return new StringValue
            {
                Value = Lambda.CurrentLambdaArgument,
            };
        }
        if (!VariableSetter.VariableValues.TryGetValue(VariableName, out Variable? var))
            throw new LanguageException($"Variable {VariableName} has never been declared", LineNumber);
        if (var.Value == VoidValue.Uninitialized) throw new LanguageException($"Variable {VariableName} hasn't been initialized yet.", LineNumber);

        // Increment/Decrement operation
        if(IsIncrementOperation){
            if(var.Value is not IntValue iv){
                throw new LanguageException($"Variables of types apart from integers can't be {(IncrementDirection == 1 ? "incremented" : "decremented")}", LineNumber);
            }
            var incrementedValue = new IntValue(){
                Value = iv.Value + IncrementDirection,
            };
            var.Value = incrementedValue;
            var.OnVariableChanged();
            return iv;
        }

        return var.Value;
    }
}