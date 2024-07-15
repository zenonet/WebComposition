namespace Core;

public class TextField : Composable
{
    public override string GenerateHtml()
    {
        if (Parameters[0].Execute() is not StringValue sv)
            throw new ("Text field only accepts strings as their first argument");
        if (Parameters[1].Execute() is not LambdaReferenceValue lambda)
            throw new ("Text field only accepts strings as their first argument");

        string id = GetNewId();
        string oninput = $"oninput=\"document.callLambdaWithArgument({lambda.FunctionIndex}, document.getElementById('{id}').value)\"";
        
        /*if (Parameters.Count > 1)
        {
            // TODO: Add lambda parameters so that this makes sense
            if (Parameters[1].Execute() is not LambdaReferenceValue lambda) throw new("Unexpected type at 2. parameter of TextField. Should be a lambda function.");
            onChange = $"onchange=\"document.callLambda({lambda.FunctionIndex})\"";
        }*/
        return $"<input id=\"{id}\" class=\"composable textfield\" type=\"text\" {oninput} value=\"{sv.Value}\">";
    }
}