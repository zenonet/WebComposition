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
        foreach (Variable var in VariableSetter.VariableValues.Values)
        {
            var.VariableChanged += Recompose;
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

    private static bool isComposing = false;
    [JSInvokable]
    public static void Recompose()
    {
        if (isComposing) return;
        isComposing = true;
        string html = Composable.ExecuteAndGetHtml(ast);
        ApplyRecomposition(html);
        isComposing = false;
    }
}