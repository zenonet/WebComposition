namespace Generator.Composables;

public class Row : Composable
{
    public override string GenerateHtml(List<Parameter> parameters, string? contentHtml = null)
    {
        return $"<div style=\"display:flex;flex-direction:row\">{contentHtml}</div>";
    }
}