namespace Generator.Composables;

public class Box : Composable
{
    public override string GenerateHtml(List<Parameter> parameters, string? contentHtml = null)
    {
        return $"<div>{contentHtml}</div>";
    }
}