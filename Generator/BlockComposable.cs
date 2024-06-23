namespace Generator;

public abstract class BlockComposable : Composable
{
    public List<Executable> Block = new();

    protected string ExecuteBlock()
    {
        return GetHtml(() =>
        {
            foreach (var t in Block)
            {
                t.Execute();
            }
        });
    }
}