using System.Text;

namespace Core;

public class StyleExtension
{
    public List<Style> Styles;
    public int LineNumber;
    public string GenerateCss()
    {
        StringBuilder sb = new();
        
        foreach (Style style in Styles)
        {
            sb.Append($"{style.PropertyName}:{style.Value.Execute().AsString(LineNumber)};");
        }
        throw new NotImplementedException("CSS generation in StyleExtensions hasn't been implemented yet.");
    }
}