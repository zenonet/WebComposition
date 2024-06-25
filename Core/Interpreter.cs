﻿using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Core;

public class Interpreter
{
    public int Line = 1;

    public Executable ParseExecutable(ref string src)
    {
        var firstExecutable = ParseExecutableWithoutExpressions(ref src);
        SkipNonStatementDelimitingWhitespace(ref src);
        // If this is not an expression
        if (src.Length == 0 || !"+-*/".Contains(src[0])) return firstExecutable;

        List<Executable> operands = new();
        List<char> operators = new();

        operands.Add(firstExecutable);

        while ("+-*/".Contains(src[0]))
        {
            SkipNonStatementDelimitingWhitespace(ref src);
            operators.Add(src[0]);
            src = src[1..];
            SkipWhitespace(ref src);
            operands.Add(ParseExecutableWithoutExpressions(ref src));
            SkipWhitespace(ref src);
        }

        Dictionary<char, int> operatorPriorities = new()
        {
            {'+', 1},
            {'-', 1},
            {'*', 2},
            {'/', 2},
        };
        
        // Now, the entire expression is contained in the two lists, we can actually start parsing 
        int currentPriority = 2; // start with the maximum priority
        while (currentPriority > 0)
        {
            for (int i = 0; i < operators.Count; i++)
            {
                int priority = operatorPriorities[operators[i]];
                if (currentPriority == priority)
                {
                    // The current operation (with operator i) can safely be merged
                    var mergedExpression = new MathExpression
                    {
                        LeftOperand = operands[i],
                        RightOperand = operands[i + 1],
                        Operator = operators[i],
                    };
                    operands[i] = mergedExpression;
                    operands.RemoveAt(i+1);
                    operators.RemoveAt(i);
                    i--;
                }
            }
            currentPriority--;
        }

        if (operands.Count > 1) throw new("There are more than one operand left from expression parsing. Something must've gone wrong. I am sorry.");

        return operands[0];
    }

    public Executable ParseExecutableWithoutExpressions(ref string src)
    {
        Match match;

        #region Parse Variable Setters

        match = Regex.Match(src, @"^([A-z]\w*)\s*=(?!=)\s*(init)?\s*");
        if (match.Success)
        {
            src = src[match.Length..];
            Executable executable = ParseExecutable(ref src);

            if (!VariableSetter.VariableValues.ContainsKey(match.Groups[1].Value))
            {
                VariableSetter.VariableValues[match.Groups[1].Value] = new();
                VariableSetter.VariableValues[match.Groups[1].Value].Value = VoidValue.Uninitialized;
            }

            return new VariableSetter
            {
                VariableName = match.Groups[1].Value,
                Value = executable,
                IsInitOnly = match.Groups[2].Success,
            };
        }

        #endregion
        
        #region Parse if statements

        match = Regex.Match(src, @"^if\s?\(");
        if (match.Success)
        {
            src = src[match.Length..];
            Executable condition = ParseExecutable(ref src);
            if (src[0] != ')') throw new("Parentheses around if statement condition aren't closed");
            src = src[1..];
            SkipWhitespace(ref src);
            
            if (src[0] != '{') throw new("Conditional block of if statement missing");
            src = src[1..];

            List<Executable> block = ParseExecutables(ref src);

            SkipWhitespace(ref src);

            if (src[0] != '}') throw new("Curly brackets around if statement conditional block aren't closed");
            src = src[1..];
            
            return new IfStatement
            {
                Condition = condition,
                Block = block,
            };
        }

        #endregion

        #region Parse function calls

        match = Regex.Match(src, @"^[^\S\r\n]*([A-z]\w*) ?[({]");
        if (match.Success)
        {
            string composableName = match.Groups[1].Value;
            src = src[(match.Length - 1)..];
            if (Function.ExecutableDefinitions.TryGetValue(composableName, out Type? executableType))
            {
                Function exe = (Function) FormatterServices.GetUninitializedObject(executableType!);

                var parameters = new List<Executable>();
                if (src[0] == '{' && exe is BlockComposable) goto block;
                src = src[1..];
                parameters = ParseParameters(ref src);
                if (src[0] != ')') throw new($"Unclosed parentheses in line {Line}");
                src = src[1..];
                exe.Parameters = parameters;
                exe.Dependencies = parameters.SelectMany(x => x.Dependencies ?? []).ToList();

                block:
                SkipWhitespace(ref src);
                // Parse content block
                if (exe is BlockComposable blockComposable)
                {
                    if (src.Length == 0 || src[0] != '{') throw new($"{composableName} is a block-composable but the call does not provide a block");
                    src = src[1..];
                    SkipWhitespace(ref src);
                    blockComposable.Block = ParseExecutables(ref src);
                    SkipWhitespace(ref src);
                    
                    if (src.Length == 0) throw new($"{composableName}'s content block isn't closed!");
                    src = src[1..];
                }

                return exe;
            }

            throw new($"Unknown composable called {composableName} in line {Line}");
        }

        #endregion

        #region Parse Lambda expressions

        match = Regex.Match(src, @"^{\s*");
        if (match.Success)
        {
            src = src[match.Length..];
            var executables = ParseExecutables(ref src);
            src = src[match.Length..];
            Lambda.FunctionDefinitions.Add(executables);
            return new Lambda
            {
                ReferenceValue = new() {FunctionIndex = Lambda.FunctionDefinitions.Count - 1},
            };
        }

        #endregion

        #region Parse bool literals

        match = Regex.Match(src, "^(true)|^false");
        if (match.Success)
        {
            src = src[match.Length..];
            return new ValueCall(new BoolValue {Value = match.Groups[1].Success});
        }

        #endregion
        
        #region Parse string literals

        match = Regex.Match(src, @"^""(.*?(?<!\\))""");
        if (match.Success)
        {
            src = src[match.Length..];
            return new ValueCall(new StringValue {Value = Regex.Replace(match.Groups[1].Value, @"\\(.)", "$1")});
        }

        #endregion

        #region Parse int literals

        match = Regex.Match(src, @"^-?\d+");
        if (match.Success)
        {
            src = src[match.Length..];
            return new ValueCall(new IntValue {Value = int.Parse(match.Value)});
        }

        #endregion

        #region Parse Variable Getters

        match = Regex.Match(src, @"^([A-z]\w*)(?:(\+\+)|(--))?");
        if (match.Success)
        {
            src = src[match.Length..];
            return new VariableGetter(match.Groups[1].Value)
            {
                IsIncrementOperation = match.Groups[2].Success || match.Groups[3].Success,
                IncrementDirection = match.Groups[2].Success ? (sbyte) 1 : (sbyte) -1,
            };
        }

        #endregion

        throw new
            ($"Invalid syntax in line {Line}");
    }

    public List<Executable> ParseExecutables(ref string src)
    {
        List<Executable> exes = new();
        SkipWhitespace(ref src);
        while (src.Length > 0 && src[0] != '}')
        {
            exes.Add(ParseExecutable(ref src));
            SkipWhitespace(ref src);
        }

        return exes;
    }

    public List<Executable> ParseParameters(ref string src)
    {
        List<Executable> parameters = new();
        while (src[0] != ')')
        {
            parameters.Add(ParseExecutable(ref src));
            SkipWhitespace(ref src);
            if (src[0] is not ')' and not ',')
                throw new($"Invalid syntax in parameter list in line {Line}");
            if (src[0] == ',') src = src[1..];
            SkipWhitespace(ref src);
        }

        return parameters;
    }

    void SkipWhitespace(ref string src)
    {
        while (src.Length > 0 && char.IsWhiteSpace(src, 0))
        {
            if (src[0] == '\n') Line++;
            src = src[1..];
        }
    }
    void SkipNonStatementDelimitingWhitespace(ref string src)
    {
        while (src.Length > 0 && char.IsWhiteSpace(src, 0) && src[0] is not '\n' and not '\r')
        {
            if (src[0] == '\n') Line++;
            src = src[1..];
        }
    }
}