using System.Collections.Generic;

namespace Generator.Composables;

public class Text : Composable
{
    public override string GenerateHtml()
    {
        var txt = Parameters[0].Execute();
        if (txt is not StringValue sv) throw new ($"The Text() composable takes a string as the parameter but received {txt.GetType().Name}");
        return $"<span class=\"composable text\">{sv.Value}</span>";
    }
}