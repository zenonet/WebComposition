namespace Core.Functions;

public class ScheduleFunction : Function
{
    public override Value Execute()
    {
        EnsureParameterCount(2);

        int delay = Parameters[0].Execute().AsInt(LineNumber).Value;
        if (Parameters[1].Execute() is not LambdaReferenceValue lambda)
            throw new LanguageException("schedule functions expects a lambda as parameter.", LineNumber);

        Task.Run(async () =>
        {
            await Task.Delay(delay, SchedulePeriodicFunction.CancellationToken);
            FunctionDefinition definition = Lambda.FunctionDefinitions[lambda.FunctionIndex];
            definition.ExecuteWithoutParameters();
        });
        return VoidValue.I;
    }
}