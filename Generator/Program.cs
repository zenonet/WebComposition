using System.Text.RegularExpressions;
using Generator;

string source = File.ReadAllText("C:\\Users\\zeno\\RiderProjects\\WebComposition\\Generator\\src.wcp");

string html = "";
int line = 1;
SkipWhitespace(ref source);    
while (source.Length != 0)
{
    html += ParseComposable(ref source);
    SkipWhitespace(ref source);
}

Console.WriteLine(html);
WriteToOutputFile(html);

return;

void SkipWhitespace(ref string src)
{
    while (src.Length > 0 && char.IsWhiteSpace(src, 0))
    {
        if (src[0] == '\n') line++;
        src = src[1..];
    }
}
string ParseComposable(ref string src)
{
    Match match = Regex.Match(src, @"^[^\S\r\n]*(\w+) ?\(?");
    if (!match.Success)
        throw new($"Invalid syntax in line {line}");

    string composableName = match.Groups[1].Value;
    src = src[match.Length..];
    if (Composable.ComposableDefinitions.TryGetValue(composableName, out Type composableType))
    {
        var parameters = new List<Parameter>();
        if(src[0] == '{') goto block;
        parameters = ParseParameters(ref src);
        if (src[0] != ')') throw new($"Unclosed parentheses in line {line}");
        src = src[1..];
        
        block:
        string? content = null;
        SkipWhitespace(ref src);
        // Parse content block
        if (src[0] == '{')
        {
            src = src[1..];
            SkipWhitespace(ref src);
            content = ParseComposables(ref src);
            SkipWhitespace(ref src);
            src = src[1..];
        }

        Composable comp = (Composable) Activator.CreateInstance(composableType)!;
        return comp.GenerateHtml(parameters, content);
    }

    throw new($"Unknown composable called {composableName} in line {line}");
}

string ParseComposables(ref string src)
{
    string html = "";
    SkipWhitespace(ref src);
    while (src.Length > 0 && src[0] != '}')
    {
        html += ParseComposable(ref src);
        SkipWhitespace(ref src);
    }

    return html;
}

List<Parameter> ParseParameters(ref string src)
{
    List<Parameter> parameters = new();
    while (src[0] != ')')
    {
        parameters.Add(ParseParameter(ref src));
        if (src[0] is not ')' and not ',')
            throw new($"Invalid syntax in parameter list in line {line}");
        if (src[0] == ',') src = src[1..];
    }

    return parameters;
}

Parameter ParseParameter(ref string src)
{
    // Parse string literal
    Match m = Regex.Match(src, @"""(.*?(?<!\\))""");
    if (m.Success)
    {
        src = src[m.Length..];
        return new() {Value = Regex.Replace(m.Groups[1].Value, @"\\(?=(.))", "$1")};
    }

    throw new($"Unable to parse parameter in line {line}");
}

void WriteToOutputFile(string html)
{
    html = "<head><link rel=\"stylesheet\" href=\"compositionStyle.css\"></head><body>" + html + "</body>";
    File.WriteAllText(@"C:\Users\zeno\RiderProjects\WebComposition\Generator\out.html", html);
}