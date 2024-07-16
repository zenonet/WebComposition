namespace Core;

public class LambdaReferenceValue : Value
{
    public override string TypeName => "lambda";

    public int FunctionIndex
    {
        get => (int)O!;
        set => O = value;
    }
}