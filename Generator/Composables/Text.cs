namespace Generator.Composables;

public class Text : Composable
{
    public override string GenerateHtml(List<Parameter> parameters, string? contentHtml = null)
    {
        return $"{parameters[0].Value}";
    }
}