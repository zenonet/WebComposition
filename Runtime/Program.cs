using System.Collections.Generic;
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
        ast = i.ParseExecutables(ref source);
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

    [JSInvokable]
    public static void Recompose()
    {
        string html = Composable.ExecuteAndGetHtml(ast);
        ApplyRecomposition(html);
    }
}