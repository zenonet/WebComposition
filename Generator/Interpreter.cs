using System.Text.RegularExpressions;

namespace Generator;

public class Interpreter
{
    public int Line = 1;

    public Executable ParseExecutable(ref string src)
    {
        #region Parse function calls

        Match match = Regex.Match(src, @"^[^\S\r\n]*([A-z]\w*) ?\(?");
        if (match.Success)
        {
            string composableName = match.Groups[1].Value;
            src = src[match.Length..];
            if (Function.ExecutableDefinitions.TryGetValue(composableName, out Type? executableType))
            {
                Function exe = (Function) Activator.CreateInstance(executableType)!;

                var parameters = new List<Executable>();
                if (src[0] == '{' && exe is BlockComposable) goto block;
                parameters = ParseParameters(ref src);
                if (src[0] != ')') throw new($"Unclosed parentheses in line {Line}");
                src = src[1..];
                exe.Parameters = parameters;

                block:
                SkipWhitespace(ref src);
                // Parse content block
                if (exe is BlockComposable blockComposable)
                {
                    if (src[0] != '{') throw new($"{composableName} is a block-composable but the call does not provide a block");
                    src = src[1..];
                    SkipWhitespace(ref src);
                    blockComposable.Block = ParseExecutables(ref src);
                    SkipWhitespace(ref src);
                    src = src[1..];
                }

                return exe;
            }

            throw new($"Unknown composable called {composableName} in line {Line}");
        }

        #endregion

        #region Parse Variable Setters

        match = Regex.Match(src, @"^([A-z]\w*)\s*=(?!=)\s*");
        if (match.Success)
        {
            src = src[match.Length..];
            return new VariableSetter
            {
                VariableName = match.Groups[1].Value,
                Value = ParseExecutable(ref src),
            };
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

        match = Regex.Match(src, @"-?\d+");
        if (match.Success)
        {
            src = src[match.Length..];
            return new ValueCall(new IntValue {Value = int.Parse(match.Value)});
        }

        #endregion
        
        #region Parse Variable Getters

        match = Regex.Match(src, @"^([A-z]\w*)");
        if (match.Success)
        {
            src = src[match.Length..];
            return new VariableGetter
            {
                VariableName = match.Groups[1].Value,
            };
        }
        
        #endregion

        throw new($"Invalid syntax in line {Line}");
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
            if (src[0] is not ')' and not ',')
                throw new($"Invalid syntax in parameter list in line {Line}");
            if (src[0] == ',') src = src[1..];
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
}