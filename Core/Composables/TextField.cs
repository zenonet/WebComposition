namespace Core;

public class TextField : Composable
{
    public override string GenerateHtml()
    {
        EnsureParameterCount(2);

        StringValue sv = Parameters[0].Execute().AsString(LineNumber);
        
        if (Parameters[1].Execute() is not LambdaReferenceValue lambda)
            throw new ("TextField expects a lambda function as it's second parameter.");

        string id = GetNewId();
        string oninput = $"oninput=\"document.callLambdaWithArgument({lambda.FunctionIndex}, document.getElementById('{id}').value, 'string')\"";
        
        return $"<input id=\"{id}\" {GetStyleStringOrEmpty()} class=\"composable textfield\" type=\"text\" {oninput} value=\"{sv.Value}\">";
    }
}