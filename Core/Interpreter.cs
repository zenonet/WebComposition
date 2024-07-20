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

        if (operands.Count > 1) throw new LanguageException("There are more than one operand left from expression parsing. Something must've gone wrong. I am sorry.", Line);

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
            if (src[0] != ':') throw new LanguageException("Ternary conditional operator without negative value (: is missing)", Line);
            src = src[1..];
            SkipWhitespace(ref src);
            Executable negative = ParseExecutable(ref src);
            return new TernaryConditionalOperator
            {
                Condition = exe,
                PositiveValue = positive,
                NegativeValue = negative,
                LineNumber = Line,
            };
        }

        return exe;
    }

    public Executable ParseExecutableWithoutExtensions(ref ReadOnlySpan<char> src)
    {
        Match match;
        SkipWhitespace(ref src);
        string srcAsString = src.ToString();

        #region Parse function definitions

        match = Regex.Match(srcAsString, @"^(?:func|(comp)) (\w+)\(");
        if (match.Success)
        {
            src = src[match.Length..];
            string functionName = match.Groups[2].Value;

            if (functionName == "Content")
                throw new LanguageException("'Content' is a reserved name. You can't name a function or composable like that", Line);

            #region Extract parameter names

            int endIndex = src.IndexOf(')');
            string[] parameterNames = Regex.Matches(src[..endIndex].ToString(), @"\s*(\w+)(?:\s*,\s*(?!\)))?")
                .Select(x => x.Groups[1].Value).ToArray();
            src = src[endIndex..];

            #endregion


            if (src[0] != ')') throw new LanguageException($"Parameter list of function definition for '{functionName}()' is not closed", Line);
            src = src[1..];

            SkipWhitespace(ref src);

            // Parse implementation block:
            if (src[0] != '{') throw new LanguageException($"Expected implementation block for function definition for '{functionName}()'", Line);
            src = src[1..];

            FunctionDefinition definition = new()
            {
                Name = functionName,
                IsComposable = match.Groups[1].Success,
                ParameterNames = parameterNames,
                Executables = ParseExecutables(ref src),
            };
            if (src[0] != '}') throw new LanguageException($"Implementation block for function '{functionName}()' isn't closed!", Line);
            src = src[1..];

            Lambda.FunctionDefinitions.Add(definition);

            return new ValueCall(VoidValue.I)
            {
                LineNumber = Line,
            };
        }

        #endregion

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
                LineNumber = Line,
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
                if (src[0] != ';') throw new LanguageException("Invalid syntax at init statement in for loop", Line);
                src = src[1..];

                Executable condition = ParseExecutable(ref src);
                SkipWhitespace(ref src);
                if (src[0] != ';') throw new LanguageException("Invalid syntax at condition statement in for loop", Line);
                src = src[1..];
                Executable incrementStatement = ParseExecutable(ref src); // TODO: Allow all parts of for loop to be empty
                SkipWhitespace(ref src);
                if (src[0] != ')') throw new LanguageException("Invalid syntax at increment statement in for loop", Line);
                src = src[1..];

                List<Executable> block = ParseBlock(ref src, "for loop");

                return new Loop
                {
                    LineNumber = Line,
                    InitStatement = initExe,
                    Condition = condition,
                    IncrementStatement = incrementStatement,
                    Block = block,
                };
            }

            if (match.Groups[1].Value is "if" or "while")
            {
                Executable condition = ParseExecutable(ref src);
                if (src[0] != ')') throw new LanguageException("Parentheses around if statement condition aren't closed", Line);
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
                        LineNumber = Line,
                        Condition = condition,
                        Block = block,
                        ElseBlock = elseBlock,
                    }
                    : new Loop
                    {
                        LineNumber = Line,
                        Condition = condition,
                        Block = block,
                    };
            }
        }

        #endregion

        #region Parse function calls

        match = Regex.Match(srcAsString, @"^[^\S\r\n]*([A-z]\w*)\s*[({]");
        if (match.Success)
        {
            string functionName = match.Groups[1].Value;
            src = src[(match.Length - 1)..];
            if (functionName == "Content")
            {
                if (!src.StartsWith("()")) throw new LanguageException("Content function can't receive parameters", Line);
                src = src[2..];
                // REFACTOR
                return new ContentComposableCall
                {
                    Definition = null!,
                    LineNumber = Line,
                };
            }

            if (Function.ExecutableDefinitions.TryGetValue(functionName, out Type? executableType))
            {
                //Function exe = (Function) FormatterServices.GetUninitializedObject(executableType)!; // This doesn't call the constructor
                Function exe = (Function) Activator.CreateInstance(executableType)!;
                exe.LineNumber = Line;
                var parameters = new List<Executable>();
                SkipWhitespace(ref src);
                if (src[0] == '{' && executableType.IsAssignableTo(typeof(BlockComposable))) goto block; // I have to use IsAssignableTo as a workaround because `is` doesn't work in wasm published builds for some reason 
                src = src[1..];
                parameters = ParseParameters(ref src);
                if (src[0] != ')') throw new LanguageException($"Unclosed parentheses", Line);
                src = src[1..];
                exe.Parameters = parameters;
                exe.Dependencies = parameters.SelectMany(x => x.Dependencies ?? []).ToList();

                block:
                SkipWhitespace(ref src);
                // Parse content block
                if (exe is BlockComposable blockComposable)
                {
                    if (src.Length == 0 || src[0] != '{') throw new LanguageException($"{functionName} is a block-composable but the call does not provide a block", Line);
                    src = src[1..];
                    SkipWhitespace(ref src);
                    blockComposable.Block = ParseExecutables(ref src);
                    SkipWhitespace(ref src);

                    if (src.Length == 0) throw new LanguageException($"{functionName}'s content block isn't closed!", Line);
                    src = src[1..];
                }
                
                if (exe is Composable composable)
                {
                    composable.StyleExtension = ParseStyleExtension(ref src);
                }
                
                return exe;
            }

            FunctionDefinition? customFunctionDefinition = Lambda.FunctionDefinitions.FirstOrDefault(x => x.Name == functionName);
            if (customFunctionDefinition != null)
            {
                List<Executable> parameters;

                if (src[0] == '{')
                {
                    parameters = [];
                    goto block;
                }

                if (!src.StartsWith("(")) throw new LanguageException($"Expected parameter list at {(customFunctionDefinition.IsComposable ? "composable" : "function")} call to '{functionName}()'", Line);

                src = src[1..];

                parameters = ParseParameters(ref src);

                if (!src.StartsWith(")")) throw new LanguageException($"Parentheses for {(customFunctionDefinition.IsComposable ? "composable" : "function")} call to '{functionName}()' are not closed", Line);
                src = src[1..];

                // I am so sorry, I am kind of too tired to do anything properly, so I just start using goto aggressively. Either I need to give up the project because of this, or I need to sped A LOT of time refactoring.
                // Kinda funny that GitHub copilot is trained on this 
                // REFACTOR
                block:
                FunctionDefinition? contentBlock = null;
                SkipWhitespace(ref src);
                // Parse content block
                if (src.Length == 0 || src[0] != '{') goto ret;
                src = src[1..];
                SkipWhitespace(ref src);
                contentBlock = new()
                {
                    Executables = ParseExecutables(ref src),
                    IsComposable = true,
                    Name = "content",
                    ParameterNames = [],
                };
                SkipWhitespace(ref src);

                if (src.Length == 0) throw new LanguageException($"{functionName}'s content block isn't closed!", Line);
                src = src[1..];
                
                ret:
                StyleExtension? extension = null;
                if (customFunctionDefinition.IsComposable)
                {
                    extension = ParseStyleExtension(ref src);
                }
                return customFunctionDefinition.IsComposable
                    ? new CustomComposableCall
                    {
                        Definition = customFunctionDefinition,
                        Parameters = parameters,
                        ContentBlock = contentBlock,
                        StyleExtension = extension,
                        LineNumber = Line,
                    }
                    : new CustomFunctionCall
                    {
                        Definition = customFunctionDefinition,
                        Parameters = parameters,
                        LineNumber = Line,
                    };
            }

            throw new LanguageException($"Unknown function called {functionName}", Line);
        }

        #endregion

        #region Parse Lambda expressions

        match = Regex.Match(srcAsString, @"^{\s*");
        if (match.Success)
        {
            src = src[match.Length..];

            FunctionDefinition definition = new()
            {
                ParameterNames = [],
                Executables = ParseExecutables(ref src),
            };
            Lambda.FunctionDefinitions.Add(definition);

            SkipWhitespace(ref src);
            if (src[0] != '}') throw new LanguageException("Lambda block isn't closed!", Line);
            src = src[1..];
            return new Lambda
            {
                ReferenceValue = new() {FunctionIndex = Lambda.FunctionDefinitions.Count - 1},
                LineNumber = Line,
            };
        }

        #endregion

        #region Parse bool literals

        match = Regex.Match(srcAsString, "^(true)|^false");
        if (match.Success)
        {
            src = src[match.Length..];
            return new ValueCall(new BoolValue {Value = match.Groups[1].Success})
            {
                LineNumber = Line,
            };
        }

        #endregion

        #region Parse string literals

        if(src[0] == '"')
        {
            src = src[1..];
            return ParseStringLiteral(ref src);
        }

        #endregion

        #region Parse int literals

        match = Regex.Match(srcAsString, @"^-?\d+");
        if (match.Success)
        {
            src = src[match.Length..];
            return new ValueCall(new IntValue {Value = int.Parse(match.Value)})
            {
                LineNumber = Line,
            };
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
                LineNumber = Line,
            };
        }

        #endregion

        throw new LanguageException
            ($"Invalid syntax", Line);
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
                throw new LanguageException("Invalid syntax in parameter list", Line);
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
        if (src[0] != '{') throw new LanguageException($"Conditional block of {statementName} statement missing", Line);
        src = src[1..];

        List<Executable> block = ParseExecutables(ref src);

        SkipWhitespace(ref src);

        if (src[0] != '}') throw new LanguageException($"Curly brackets around {statementName} statement conditional block aren't closed", Line);
        src = src[1..];
        return block;
    }

    private readonly string[] cssUnits = ["cm", "mm", "in", "px", "pt", "pc", "em", "ex", "ch", "rem", "vw", "vh", "vmin", "vmax", "%"];
    StyleExtension? ParseStyleExtension(ref ReadOnlySpan<char> src)
    {
        if (!src.StartsWith(".styled")) return null;
        if (src[7] != '{') throw new LanguageException("Expected style block after .styled extension", Line);
        src = src[8..];
        StyleExtension extension = new();
        
        while (src.Length > 0 && src[0] != '}')
        {
            int colonIndex = src.IndexOf(':');
            ReadOnlySpan<char> propertyName = src[..colonIndex].Trim();
            src = src[(colonIndex + 1)..];
            SkipWhitespace(ref src);
            Executable exe = ParseExecutable(ref src);
            
            SkipWhitespace(ref src);
            
            string unit = "";
            // Allow css units after expression
            foreach (string u in cssUnits)
            {
                if (!src.StartsWith(u)) continue;
                src = src[u.Length..];
                unit = u;
                break;
            }
            
            extension.Styles.Add(new()
            {
                PropertyName = propertyName.ToString(),
                Value = exe,
                Unit = unit,
            });
            
            SkipWhitespace(ref src);
            if (src[0] is not ',' and not '}')
                throw new LanguageException("Invalid syntax at style extension", Line);
            if (src[0] is ',')
                src = src[1..];
            SkipWhitespace(ref src);
            if (src[0] is '}')
            {
                src = src[1..];
                break;
            }
        }
        return extension;
    }

    Executable ParseStringLiteral(ref ReadOnlySpan<char> src, Executable? expression = null)
    {
        int length = 0;
        bool isEscaped = false;
        while (isEscaped || src[length] != '"')
        {
            if (!isEscaped && src[length] == '{')
            {
                AddLiteralToExpression(ref src, ref expression, length);
                src = src[1..];
                // Add the argument to the concatenation expression
                var argument = ParseExecutable(ref src);
                if (src[0] != '}') throw new LanguageException("Unclosed string interpolation", Line);
                src = src[1..];

                expression = new Expression
                {
                    LeftOperand = expression,
                    RightOperand = argument,
                };
                // Recursively continue parsing the string
                expression = ParseStringLiteral(ref src, expression);
                return expression;
            }
                
            if (isEscaped) isEscaped = false;
            if (!isEscaped && src[length] == '\\')
            {
                isEscaped = true;
            }
            
            length++;
        }

        AddLiteralToExpression(ref src, ref expression, length);
        src = src[1..];

        return expression;

        void AddLiteralToExpression(ref ReadOnlySpan<char> readOnlySpan, ref Executable? executable, int len)
        {
            string value = new Regex(@"\\(.)").Replace(readOnlySpan[..len].ToString(), "$1");
            ValueCall literalValueCall = new (new StringValue {Value = value}){LineNumber = Line};
            if (executable == null)
            {
                executable = literalValueCall;
            }
            else
            {
                // Add the argument to the concatenation expression
                executable = new Expression
                {
                    LeftOperand = executable,
                    RightOperand = literalValueCall,
                };
            }

            readOnlySpan = readOnlySpan[len..];
        }
    }
    
}