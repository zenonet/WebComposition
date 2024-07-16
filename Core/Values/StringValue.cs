namespace Core;

public class StringValue : Value
{
    public override string TypeName => "string";

    public string Value
    {
        get => (string)O!;
        set => O = value;
    }
}