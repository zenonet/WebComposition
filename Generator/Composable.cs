﻿using System.Text;

namespace Generator;

public abstract class Composable : Function
{
    public abstract string GenerateHtml();

    public override Value Execute()
    {
        composableBlockStack.Peek().Append(GenerateHtml());
        return null!;
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