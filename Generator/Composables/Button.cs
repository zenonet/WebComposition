using System.Collections.Generic;

namespace Generator.Composables;

public class Button : BlockComposable
{
    public override string GenerateHtml()
    {
        string onClick = "";
        if (Parameters.Count > 0)
        {
            var onClickFunction = Parameters[0].Execute();
            if (onClickFunction is LambdaReferenceValue lrv)
            {
                onClick = $"onclick=\"Program.callLambda({lrv.FunctionIndex})\"";
            }
        }

        return $"<button class=\"button composable\" {onClick}>{ExecuteBlock()}</button>";
    }
}