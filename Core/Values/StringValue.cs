namespace Core;

public class StringValue : Value
{
    public string Value
    {
        get => (string)O!;
        set => O = value;
    }
}