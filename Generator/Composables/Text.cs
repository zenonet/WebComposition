using System.Collections.Generic;

namespace Generator.Composables;

public class Text : Composable
{
    public override string GenerateHtml(List<Parameter> parameters, string? contentHtml = null)
    {
        return $"<span class=\"composable text\">{parameters[0].Value}</span>";
    }
}