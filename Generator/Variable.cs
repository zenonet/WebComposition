namespace Generator;

public class Variable
{
    public Value Value;
    public event Action VariableChanged = null!;

    public virtual void OnVariableChanged()
    {
        VariableChanged?.Invoke();
    }
}