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
        if (!VariableSetter.VariableValues.TryGetValue(VariableName, out Variable? var))
            throw new($"Variable {VariableName} has never been declared");
        if (var.Value == VoidValue.Uninitialized) throw new($"Variable {VariableName} hasn't been initialized yet.");

        // Increment/Decrement operation
        if(IsIncrementOperation){
            if(var.Value is not IntValue iv){
                throw new($"Variables of types apart from integers can't be {(IncrementDirection == 1 ? "incremented" : "decremented")}");
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