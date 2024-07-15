using Core.Composables;

namespace Core;

public abstract class Function : Executable
{
    public List<Executable> Parameters = new();
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
        {"TextField", typeof(TextField)},
    };
}