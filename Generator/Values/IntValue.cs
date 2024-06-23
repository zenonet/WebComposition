namespace Generator;

public class IntValue : Value
{
    public int Value
    {
        get => (int)O!;
        set => O = value;
    }
}