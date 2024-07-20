namespace Core.Functions;

public class SchedulePeriodicFunction : Function
{
    public static CancellationToken CancellationToken;
    public bool IsActive;
    public override Value Execute()
    {
        EnsureParameterCount(2);

        int delay = Parameters[0].Execute().AsInt(LineNumber).Value;
        if (Parameters[1].Execute() is not LambdaReferenceValue lambda)
            throw new LanguageException("schedulePeriodic functions expects a lambda as parameter.", LineNumber);
        
        if (IsActive) return VoidValue.I;
        Task.Run(async () =>
        {
            while (!CancellationToken.IsCancellationRequested)
            {
                await Task.Delay(delay, CancellationToken);
                FunctionDefinition definition = Lambda.FunctionDefinitions[lambda.FunctionIndex];
                definition.ExecuteWithoutParameters();
            }
        });
        IsActive = true;
        return VoidValue.I;
    }
}