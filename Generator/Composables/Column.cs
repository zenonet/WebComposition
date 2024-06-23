using System.Collections.Generic;

namespace Generator.Composables;

public class Column : Composable
{
    public override string GenerateHtml(List<Parameter> parameters, string? contentHtml = null)
    {
        return $"<div class=\"column composable container\" style=\"display:flex;flex-direction:column\">{contentHtml}</div>";
    }
}