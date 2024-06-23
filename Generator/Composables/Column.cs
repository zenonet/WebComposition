namespace Generator.Composables;

public class Column : Composable
{
    public override string GenerateHtml(List<Parameter> parameters, string? contentHtml = null)
    {
        return $"<div style=\"display:flex;flex-direction:column\">{contentHtml}</div>";
    }
}