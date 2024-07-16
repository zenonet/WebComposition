namespace Core;

public class IntValue : Value
{
    public override string TypeName => "int";

    public int Value
    {
        get => (int)O!;
        set => O = value;
    }
}