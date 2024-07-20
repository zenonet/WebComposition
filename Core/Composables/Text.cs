
namespace Core.Composables;

public class Text : Composable
{
    public override string GenerateHtml()
    {
        EnsureParameterCount(1);
        
        StringValue sv = Parameters[0].Execute().AsString(LineNumber);
        
        return $"<span class=\"composable text\" {GetStyleStringOrEmpty()}>{sv.Value.Replace("\\n", "<br>")}</span>";
    }
}