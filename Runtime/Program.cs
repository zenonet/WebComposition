using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Bootsharp;
using Core;
using Core.Functions;

public static partial class Program
{
    public static Interpreter i = new();
    private static List<Executable> ast;
    private static CancellationTokenSource cancellationTokenSource = new();
    public static void Main ()
    {
    }

    [JSInvokable]
    public static void SetSourcecode(string source)
    {
        VariableSetter.VariableValues.Clear();
        Lambda.FunctionDefinitions.Clear();
        cancellationTokenSource.Cancel();

        i.Line = 1;

        // TODO: Request cancellation of periodic calls
        try
        {
            Stopwatch sw = Stopwatch.StartNew();
            ast = i.ParseExecutables(source);
            sw.Stop();
            Log($"Parsing took {sw.Elapsed.TotalMilliseconds}ms");
        }
        catch (LanguageException e)
        {
            ShowError($"Error {(e.LineNumber != -1 ? $"in line {e.LineNumber}" : "")}: " + e.Message);
            return;
        }
        catch (Exception e)
        {
            ShowError("Internal Error:" + e.Message);
            return;
        }
        
        foreach (KeyValuePair<string, Variable> var in VariableSetter.VariableValues)
        {
            var.Value.VariableChanged += Recompose;
        }

        cancellationTokenSource.Dispose();
        cancellationTokenSource = new();
        SchedulePeriodicFunction.CancellationToken = cancellationTokenSource.Token;
        Recompose();
    }

    [JSFunction]
    public static partial void Log (string msg);
    
    [JSFunction] // Set in JS as Program.applyRecomposition = () => ..
    public static partial void ApplyRecomposition (string newBody); 
    
    [JSFunction]
    public static partial void ShowError (string message);

    [JSInvokable]
    public static void CallLambda(int id)
    {
        FunctionDefinition lambda = Lambda.FunctionDefinitions[id];
        foreach (Executable e in lambda.Executables)
        {
            e.Execute();
        }
    }
    [JSInvokable]
    public static void CallLambdaWithArgument(int id, string argument, string type)
    {
        FunctionDefinition lambda = Lambda.FunctionDefinitions[id];
        Lambda.CurrentLambdaArgument = type switch
        {
            "bool" => new BoolValue {Value = argument == "true"},
            "string" => new StringValue {Value = argument},
            "int" => new IntValue {Value = int.Parse(argument)},
        };
        foreach (Executable e in lambda.Executables)
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
        try
        {
            Composable.RecompositionCounter++;
            Stopwatch sw = Stopwatch.StartNew();
            string html = Composable.ExecuteAndGetHtml(ast);
            sw.Stop();
            Log($"Recomposition took {sw.Elapsed.TotalMilliseconds}ms");
            ApplyRecomposition(html);
        }
        catch (LanguageException e)
        {
            ShowError($"Error in line {e.LineNumber}: " + e.Message);
        }
        catch (Exception e)
        {
            ShowError("Error:" + e.Message);
        }

        isComposing = false;
    }
}