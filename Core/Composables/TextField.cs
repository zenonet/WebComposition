namespace Core;

public class TextField : Composable
{
    public override string GenerateHtml()
    {
        EnsureParameterCount(2);
        
        Value txt = Parameters[0].Execute();
        if (txt is IntValue iv) txt = new StringValue{Value = iv.Value.ToString()};
        if (txt is not StringValue sv) throw new ($"The TextField() composable takes a string as it's first parameter but received {txt.TypeName}");
        
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
        return $"<input id=\"{id}\" {GetStyleStringOrEmpty()} class=\"composable textfield\" type=\"text\" {oninput} value=\"{sv.Value}\">";
    }
}