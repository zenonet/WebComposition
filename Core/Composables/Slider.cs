namespace Core.Composables;

public class Slider : Composable
{
    public override string GenerateHtml()
    {
        EnsureParameterCount(4);

        IntValue min = Parameters[0].Execute().AsInt(LineNumber);
        IntValue max = Parameters[1].Execute().AsInt(LineNumber);
        IntValue value = Parameters[2].Execute().AsInt(LineNumber);
        LambdaReferenceValue lambda = Parameters[3].Execute().AsLambda(LineNumber);

        string id = GetNewId();
        string oninput = $"oninput=\"document.callLambdaWithArgument({lambda.FunctionIndex}, document.getElementById('{id}').value, 'int')\"";
                
        return $"<input id=\"{id}\" type=\"range\" min=\"{min.Value}\" max=\"{max.Value}\" value=\"{value.Value}\" {GetStyleStringOrEmpty()} class=\"composable slider\" {oninput}>";
    }
}