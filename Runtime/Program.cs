using System.Collections.Generic;
using System.Diagnostics;
using Bootsharp;
using Core;
public static partial class Program
{
    public static Interpreter i = new();
    private static List<Executable> ast;
    public static void Main ()
    {
    }

    [JSInvokable]
    public static void SetSourcecode(string source)
    {
        Log("Source: " + source);
        VariableSetter.VariableValues.Clear();
        Lambda.FunctionDefinitions.Clear();
        i.Line = 1;

        Stopwatch sw = Stopwatch.StartNew();
        ast = i.ParseExecutables(source);
        sw.Stop();
        Log($"Parsing took {sw.Elapsed.TotalMilliseconds}ms");
        
        foreach (KeyValuePair<string, Variable> var in VariableSetter.VariableValues)
        {
            var.Value.VariableChanged += Recompose;
        }
        Recompose();
    }

    [JSFunction]
    public static partial void Log (string msg);
    
    [JSFunction] // Set in JS as Program.applyRecomposition = () => ..
    public static partial void ApplyRecomposition (string newBody);

    [JSInvokable]
    public static void CallLambda(int id)
    {
        List<Executable> lambda = Lambda.FunctionDefinitions[id];
        foreach (Executable e in lambda)
        {
            e.Execute();
        }
    }

    private static bool isComposing;
    [JSInvokable]
    public static void Recompose()
    {
        if (isComposing)
        {
            // Log("Recomposition rejected because the composition process isn't over yet");
            return;
        }
        isComposing = true;
        Stopwatch sw = Stopwatch.StartNew();
        string html = Composable.ExecuteAndGetHtml(ast);
        sw.Stop();
        Log($"Recomposition took {sw.Elapsed.TotalMilliseconds}ms");
        ApplyRecomposition(html);
        isComposing = false;
    }
}