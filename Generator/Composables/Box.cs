using System.Collections.Generic;

namespace Generator.Composables;

public class Box : Composable
{
    public override string GenerateHtml(List<Parameter> parameters, string? contentHtml = null)
    {
        return $"<div class=\"box composable container\">{contentHtml}</div>";
    }
}