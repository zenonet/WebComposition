﻿using System.Text;

namespace Core;

public class StyleExtension
{
    public List<Style> Styles = new();
    public int LineNumber;
    public string GenerateCss()
    {
        StringBuilder sb = new();
        
        foreach (Style style in Styles)
        {
            sb.Append($"{style.PropertyName}:{style.Value.Execute().AsString(LineNumber).Value}{style.Unit};");
        }

        return sb.ToString();
    }
}