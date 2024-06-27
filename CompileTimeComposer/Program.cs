
using System.Diagnostics;
using Core;

Interpreter i = new();
string src = File.ReadAllText("../../../../Core/src.wcp");


Stopwatch sw = Stopwatch.StartNew();
var exes = i.ParseExecutables(src);
sw.Stop();
Console.WriteLine($"Parsing took {sw.Elapsed.TotalMilliseconds}ms");

foreach ((string? key, Variable? value) in VariableSetter.VariableValues)
{
    value.VariableChanged += (() =>
    {
        Console.WriteLine($"Variable {key} was changed to {value.Value}");
    });
}
sw = Stopwatch.StartNew();
string html = Composable.ExecuteAndGetHtml(exes);
sw.Stop();
Console.WriteLine($"Composition took {sw.Elapsed.TotalMilliseconds}ms");
File.WriteAllText("../../../../Core/out.html", html);
Console.WriteLine(html);
