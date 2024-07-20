using System.Collections.Generic;

namespace Core.Composables;

public class Box : BlockComposable
{
    public override string GenerateHtml()
    {
        return $"<div class=\"box composable container\" {GetStyleStringOrEmpty()}>{ExecuteAndGetHtml([..Block])}</div>";
    }
}