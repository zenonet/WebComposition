namespace Generator.Composables;

public class Row : Composable
{
    public override string GenerateHtml(List<Parameter> parameters, string? contentHtml = null)
    {
        return $"<div class=\"row composable container\" style=\"display:flex;flex-direction:row\">{contentHtml}</div>";
    }
}