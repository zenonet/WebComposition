using System.Buffers;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Core;

public class Interpreter
{
    public int Line = 1;

    private OperatorType GetNextOperator(ref ReadOnlySpan<char> src)
    {
        if (src.StartsWith(">=") || src.StartsWith("<=") || src.StartsWith("=="))
        {
            ReadOnlySpan<char> op = src[..2];
            src = src[2..];
            return op switch
            {
                ">=" => OperatorType.GreaterThanOrEqualTo,
                "<=" => OperatorType.LessThanOrEqualTo,
                "==" => OperatorType.Equals,
            };
        }

        if ("+-*/<>".Contains(src[0]) && src[1] != '/') // && is a quick fix for parse detecting comments as divide operations
        {
            char op = src[0];
            src = src[1..];
            return op switch
            {
                '>' => OperatorType.GreaterThan,
                '<' => OperatorType.LessThan,
                '+' => OperatorType.Plus,
                '-' => OperatorType.Minus,
                '*' => OperatorType.Multiply,
                '/' => OperatorType.Divide,
            };
        }

        return OperatorType.None;
    }

    public Executable ParseExecutable(ref ReadOnlySpan<char> src)
    {
        var firstExecutable = ParseExecutableWithoutExpressions(ref src);
        SkipNonStatementDelimitingWhitespace(ref src);
        if (src.Length == 0) return firstExecutable;

        SkipNonStatementDelimitingWhitespace(ref src);

        // If this is not an expression
        OperatorType o = GetNextOperator(ref src);
        if (o == OperatorType.None) return firstExecutable;

        List<Executable> operands = new();
        List<OperatorType> operators = new();

        operands.Add(firstExecutable);
        operators.Add(o);


        while (true)
        {
            SkipNonStatementDelimitingWhitespace(ref src);
            operands.Add(ParseExecutableWithoutExpressions(ref src));
            SkipNonStatementDelimitingWhitespace(ref src);

            SkipNonStatementDelimitingWhitespace(ref src);

            OperatorType? op = GetNextOperator(ref src);
            if (op == OperatorType.None) break;

            operators.Add(op.Value);
        }

        Dictionary<OperatorType, int> operatorPriorities = new()
        {
            {OperatorType.Plus, 2},
            {OperatorType.Minus, 2},
            {OperatorType.Multiply, 3},
            {OperatorType.Divide, 3},

            {OperatorType.Equals, 1},
            {OperatorType.GreaterThan, 1},
            {OperatorType.GreaterThanOrEqualTo, 1},
            {OperatorType.LessThan, 1},
            {OperatorType.LessThanOrEqualTo, 1},
        };

        // Now, the entire expression is contained in the two lists, we can actually start parsing 
        int currentPriority = 3; // start with the maximum priority
        while (currentPriority > 0)
        {
            for (int i = 0; i < operators.Count; i++)
            {
                int priority = operatorPriorities[operators[i]];
                if (currentPriority == priority)
                {
                    // The current operation (with operator i) can safely be merged
                    var mergedExpression = new Expression
                    {
                        LeftOperand = operands[i],
                        RightOperand = operands[i + 1],
                        Operator = operators[i],
                    };
                    operands[i] = mergedExpression;
                    operands.RemoveAt(i + 1);
                    operators.RemoveAt(i);
                    i--;
                }
            }

            currentPriority--;
        }

        if (operands.Count > 1) throw new("There are more than one operand left from expression parsing. Something must've gone wrong. I am sorry.");

        return operands[0];
    }

    public Executable ParseExecutableWithoutExpressions(ref ReadOnlySpan<char> src)
    {
        Executable exe = ParseExecutableWithoutExtensions(ref src);
        SkipNonStatementDelimitingWhitespace(ref src);
        if (src.Length > 0 && src[0] == '?')
        {
            src = src[1..];
            SkipWhitespace(ref src);
            // Ternary conditional operator
            Executable positive = ParseExecutable(ref src);
            SkipWhitespace(ref src);
            if (src[0] != ':') throw new("Ternary conditional operator without negative value (: is missing)");
            src = src[1..];
            SkipWhitespace(ref src);
            Executable negative = ParseExecutable(ref src);
            return new TernaryConditionalOperator
            {
                Condition = exe,
                PositiveValue = positive,
                NegativeValue = negative,
            };
        }

        return exe;
    }

    public Executable ParseExecutableWithoutExtensions(ref ReadOnlySpan<char> src)
    {
        Match match;
        SkipWhitespace(ref src);
        string srcAsString = src.ToString();
        
        #region Parse Variable Setters

        match = Regex.Match(srcAsString, @"^([A-z]\w*)\s*=(?!=)\s*(init)?\s*");
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

        match = Regex.Match(srcAsString, @"^(if|while|for)\s?\(");
        if (match.Success)
        {
            src = src[match.Length..];

            if (match.Groups[1].Value == "for")
            {
                // Parse for loop
                Executable initExe = ParseExecutable(ref src);
                SkipWhitespace(ref src);
                if (src[0] != ';') throw new("Invalid syntax at init statement in for loop");
                src = src[1..];

                Executable condition = ParseExecutable(ref src);
                SkipWhitespace(ref src);
                if (src[0] != ';') throw new("Invalid syntax at condition statement in for loop");
                src = src[1..];
                Executable incrementStatement = ParseExecutable(ref src); // TODO: Allow all parts of for loop to be empty
                SkipWhitespace(ref src);
                if (src[0] != ')') throw new("Invalid syntax at increment statement in for loop");
                src = src[1..];

                List<Executable> block = ParseBlock(ref src, "for loop");

                return new Loop
                {
                    InitStatement = initExe,
                    Condition = condition,
                    IncrementStatement = incrementStatement,
                    Block = block,
                };
            }

            if (match.Groups[1].Value is "if" or "while")
            {
                Executable condition = ParseExecutable(ref src);
                if (src[0] != ')') throw new("Parentheses around if statement condition aren't closed");
                src = src[1..];
                SkipWhitespace(ref src);


                List<Executable>? elseBlock = null;
                List<Executable> block = ParseBlock(ref src, "if");

                SkipWhitespace(ref src);

                if (match.Groups[1].Value == "if" && src.StartsWith("else"))
                {
                    src = src[4..];
                    elseBlock = ParseBlock(ref src, "else");
                }

                return match.Groups[1].Value == "if"
                    ? new IfStatement
                    {
                        Condition = condition,
                        Block = block,
                        ElseBlock = elseBlock,
                    }
                    : new Loop
                    {
                        Condition = condition,
                        Block = block,
                    };
            }
        }

        #endregion

        #region Parse function calls

        match = Regex.Match(srcAsString, @"^[^\S\r\n]*([A-z]\w*) ?[({]");
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

        match = Regex.Match(srcAsString, @"^{\s*");
        if (match.Success)
        {
            src = src[match.Length..];
            var executables = ParseExecutables(ref src);
            SkipWhitespace(ref src);
            if (src[0] != '}') throw new("Lambda block isn't closed!");
            src = src[1..];
            Lambda.FunctionDefinitions.Add(executables);
            return new Lambda
            {
                ReferenceValue = new() {FunctionIndex = Lambda.FunctionDefinitions.Count - 1},
            };
        }

        #endregion

        #region Parse bool literals

        match = Regex.Match(srcAsString, "^(true)|^false");
        if (match.Success)
        {
            src = src[match.Length..];
            return new ValueCall(new BoolValue {Value = match.Groups[1].Success});
        }

        #endregion

        #region Parse string literals

        match = Regex.Match(srcAsString, @"^""(.*?(?<!\\))""");
        if (match.Success)
        {
            src = src[match.Length..];
            return new ValueCall(new StringValue {Value = Regex.Replace(match.Groups[1].Value, @"\\(.)", "$1")});
        }

        #endregion

        #region Parse int literals

        match = Regex.Match(srcAsString, @"^-?\d+");
        if (match.Success)
        {
            src = src[match.Length..];
            return new ValueCall(new IntValue {Value = int.Parse(match.Value)});
        }

        #endregion

        #region Parse Variable Getters

        match = Regex.Match(srcAsString, @"^([A-z]\w*)(?:(\+\+)|(--))?");
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

    public List<Executable> ParseExecutables(string src)
    {
        ReadOnlySpan<char> charSpan = src.AsSpan();
        return ParseExecutables(ref charSpan);
    }
    public List<Executable> ParseExecutables(ref ReadOnlySpan<char> src)
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

    public List<Executable> ParseParameters(ref ReadOnlySpan<char> src)
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

    void SkipWhitespace(ref ReadOnlySpan<char> src)
    {
        bool isInComment = false;
        while (src.Length > 0 && (char.IsWhiteSpace(src[0]) || isInComment) || src.StartsWith("//"))
        {
            if (src.StartsWith("//"))
            {
                isInComment = true;
                src = src[2..];
                continue;
            }
            
            if (src[0] == '\n')
            {
                Line++;
                isInComment = false;
            }
            src = src[1..];
        }
    }

    void SkipNonStatementDelimitingWhitespace(ref ReadOnlySpan<char> src)
    {
        while (src.Length > 0 && char.IsWhiteSpace(src[0]) && src[0] is not '\n' and not '\r')
        {
            if (src[0] == '\n') Line++;
            src = src[1..];
        }
    }

    List<Executable> ParseBlock(ref ReadOnlySpan<char> src, string statementName)
    {
        if (src[0] != '{') throw new($"Conditional block of {statementName} statement missing");
        src = src[1..];

        List<Executable> block = ParseExecutables(ref src);

        SkipWhitespace(ref src);

        if (src[0] != '}') throw new($"Curly brackets around {statementName} statement conditional block aren't closed");
        src = src[1..];
        return block;
    }
}