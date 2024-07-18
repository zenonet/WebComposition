namespace Core;

public class ContentComposableCall : CustomComposableCall
{
    public static Stack<FunctionDefinition> ContentFunctionStack = new();

    public ContentComposableCall()
    {
        Definition = null!;
    }
    public override string GenerateHtml()
    {
        Definition = ContentFunctionStack.Peek();
        return base.GenerateHtml();
    }
}