using System.Collections.Generic;

namespace Generator.Composables;

public class Box : BlockComposable
{
    public override string GenerateHtml()
    {
        return $"<div class=\"box composable container\">{ExecuteAndGetHtml([..Block])}</div>";
    }
}