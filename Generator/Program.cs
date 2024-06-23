
using Generator;

string source = File.ReadAllText("../../../src.wcp");

Interpreter i = new();
var ast = i.ParseExecutables(ref source);
string html = Composable.ExecuteAndGetHtml(ast);
Console.WriteLine(html);
WriteToOutputFile(html);
return;



void WriteToOutputFile(string html)
{
    html = File.ReadAllText("../../../base.html").Replace("{{{Content}}}", html);
    File.WriteAllText("../../../out.html", html);
}