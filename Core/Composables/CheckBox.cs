namespace Core.Composables;

public class CheckBox : Composable
{
    public override string GenerateHtml()
    {
        EnsureParameterCount(2);
        
        BoolValue isChecked = Parameters[0].Execute().AsBool(LineNumber);
        
        if (Parameters[1].Execute() is not LambdaReferenceValue lambda)
            throw new ("CheckBox expects a lambda function as it's second parameter.");

        string id = GetNewId();
        string oninput = $"oninput=\"document.callLambdaWithArgument({lambda.FunctionIndex}, document.getElementById('{id}').checked)\"";
                
        return $"<input id=\"{id}\" type=\"checkbox\" {GetStyleStringOrEmpty()} class=\"composable textfield\" {oninput} {(isChecked.Value ? "checked" : "")}>";
    }
}