namespace Core;

public class BoolValue : Value
{
    public bool Value
    {
        get => (bool)O!;
        set => O = value;
    }
}