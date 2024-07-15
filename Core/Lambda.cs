namespace Core;

public class Lambda : Executable
{
    public static List<List<Executable>> FunctionDefinitions = new();
    public LambdaReferenceValue ReferenceValue;
    public static string? CurrentLambdaArgument;
    public override Value Execute()
    {
        return ReferenceValue;
    }
}