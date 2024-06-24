namespace Core;

public class ValueCall(Value value) : Executable
{
    public override Value Execute() => value;
}