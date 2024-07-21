using Core.Composables;
using Core.Functions;

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
        {"CheckBox", typeof(CheckBox)},
        {"Slider", typeof(Slider)},
        
        {"int", typeof(IntConversionFunction)},
        
        {"schedulePeriodic", typeof(SchedulePeriodicFunction)},
        {"schedule", typeof(ScheduleFunction)},
    };

    public string GetFunctionName()
    {
        return ExecutableDefinitions.FirstOrDefault(x => x.Value == this.GetType()).Key;
    }
    
    protected void EnsureParameterCount(int count)
    {
        if (Parameters.Count != count)
            throw new LanguageException($"The {GetFunctionName()} function takes {count} parameters but received {Parameters.Count}", LineNumber);
    }
    protected void EnsureParameterCount(int min, int max)
    {
        if (Parameters.Count < min || Parameters.Count > max)
            throw new LanguageException($"The {GetFunctionName()} function takes between {min} and {max} parameters but received {Parameters.Count}", LineNumber);
    }
}