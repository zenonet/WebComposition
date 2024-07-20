
namespace Core.Composables;

public class Text : Composable
{
    public override string GenerateHtml()
    {
        EnsureParameterCount(1);
        
        var txt = Parameters[0].Execute();
        if (txt is IntValue iv) txt = new StringValue{Value = iv.Value.ToString()};
        if (txt is not StringValue sv) throw new ($"The Text() composable takes a string as the parameter but received {txt.TypeName}");
        return $"<span class=\"composable text\" {GetStyleStringOrEmpty()}>{sv.Value.Replace("\\n", "<br>")}</span>";
    }
}