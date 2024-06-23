using System.Collections.Generic;

namespace Generator.Composables;

public class Row : BlockComposable
{
    public override string GenerateHtml()
    {
        return $"<div class=\"row composable container\" style=\"display:flex;flex-direction:row\">{ExecuteBlock()}</div>";
    }
}