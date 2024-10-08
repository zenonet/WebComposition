﻿using System.Text;

namespace Core;

public abstract class Composable : Function
{
    public static int RecompositionCounter = 0;
    
    public static int callIdCounter = 0;
    private int instanceIdCounter;
    
    private int callId = 0;
    
    private int lastRecomposition = -1;

    public StyleExtension? StyleExtension = null;
    protected string GetNewId()
    {
        if (lastRecomposition != RecompositionCounter)
        {
            // TODO: Shouldn't happen on every recomposition but on every parsing
            instanceIdCounter = 0;
            lastRecomposition = RecompositionCounter;
        }
        return $"{callId}_{instanceIdCounter++}";
    }

    protected string GetStyleStringOrEmpty()
    {
        if (StyleExtension == null) return "";
        return $"style=\"{StyleExtension?.GenerateCss()}\"";
    }

    public Composable()
    {
        callId = callIdCounter++;
        Console.WriteLine($"Initializing new composable. Call id: {callId}");
    }

    public abstract string GenerateHtml();

    public override Value Execute()
    {
        if (composableBlockStack.Count < 1) throw new ($"Empty block stack, can't compose {GetFunctionName()}");
        composableBlockStack.Peek().Append(GenerateHtml());
        return VoidValue.I;
    }

    private static Stack<StringBuilder> composableBlockStack = new();
    public static string ExecuteAndGetHtml(List<Executable> exes)
    {
        StringBuilder sb = new();
        composableBlockStack.Push(sb);
        foreach (Executable c in exes)
        {
            c.Execute();
        }
        composableBlockStack.Pop();
        return sb.ToString();
    }
    protected static string GetHtml(Action a)
    {
        StringBuilder sb = new();
        composableBlockStack.Push(sb);
        a();
        composableBlockStack.Pop();
        return sb.ToString();
    }
    
}