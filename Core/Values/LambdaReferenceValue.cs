namespace Core;

public class LambdaReferenceValue : Value
{
    public int FunctionIndex
    {
        get => (int)O!;
        set => O = value;
    }
}