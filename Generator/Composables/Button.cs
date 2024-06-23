using System.Collections.Generic;

namespace Generator.Composables;

public class Button : BlockComposable
{
    public override string GenerateHtml()
    {
        return $"<button class=\"button composable\">{ExecuteBlock()}</button>";
    }
}