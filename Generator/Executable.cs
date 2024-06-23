using Generator.Composables;

namespace Generator;

public abstract class Executable
{
    public List<Executable> Parameters = new();
    public abstract Value Execute();

    public Value[] EvaluateParameterValues()
    {
        Value[] values = new Value[Parameters.Count];
        for (var i = 0; i < Parameters.Count; i++)
        {
            values[i] = Parameters[i].Execute();
        }
        return values;
    }

    public static Dictionary<string, Type> ExecutableDefinitions = new()
    {
        {"Text", typeof(Text)},
        {"Box", typeof(Box)},
        {"Column", typeof(Column)},
        {"Row", typeof(Row)},
        {"Button", typeof(Button)},
    };
}