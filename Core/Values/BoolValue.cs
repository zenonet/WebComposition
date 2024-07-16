namespace Core;

public class BoolValue : Value
{
    public override string TypeName => "bool";

    public bool Value
    {
        get => (bool)O!;
        set => O = value;
    }
}