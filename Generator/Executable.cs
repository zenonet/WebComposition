using Generator.Composables;

namespace Generator;

public abstract class Executable
{
    /// <summary>
    /// List of Variables this executable is dependent on
    /// </summary>
    public List<string>? Dependencies = null;
    public abstract Value Execute();
}