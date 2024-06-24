using System.Collections.Generic;

namespace Core.Composables;

public class Row : BlockComposable
{
    public override string GenerateHtml()
    {
        return $"<div class=\"row composable container\" style=\"display:flex;flex-direction:row\">{ExecuteBlock()}</div>";
    }
}