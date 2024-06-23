using System.Collections.Generic;

namespace Generator.Composables;

public class Button : Composable
{
    public override string GenerateHtml(List<Parameter> parameters, string? contentHtml = null)
    {
        return $"<button class=\"button composable\">{contentHtml}</button>";
    }
}