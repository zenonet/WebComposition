using System.Collections.Generic;

namespace Core.Composables;

public class Button : BlockComposable
{
    public override string GenerateHtml()
    {
        string onClick = "";
        if (Parameters is {Count: > 0})
        {
            var onClickFunction = Parameters[0].Execute();
            if (onClickFunction is LambdaReferenceValue lrv)
            {
                onClick = $"onclick=\"document.callLambda({lrv.FunctionIndex})\"";
            }
        }

        return $"<button class=\"button composable\" {GetStyleStringOrEmpty()} {onClick}>{ExecuteBlock()}</button>";
    }
}