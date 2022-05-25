using MongoDB.Driver;
using System.Linq.Expressions;

namespace AshLake.Services.Archiver.Infrastructure.Extensions;
public static class HangfireBackgroundJob
{
    public static IEnumerable<string> EnqueueSuccessively<T>(this IBackgroundJobClient client, List<Expression<Func<T, Task>>> methodCalls)
    {
        Guard.Against.Null(methodCalls);
        if (methodCalls.Count == 0) yield break;

        var jobId = client.Enqueue(methodCalls.First());
        yield return jobId;

        if (methodCalls.Count == 1) yield break;

        foreach (var methodCall in methodCalls.Skip(1))
        {
            jobId = client.ContinueJobWith(jobId, methodCall,null,JobContinuationOptions.OnAnyFinishedState);
            yield return jobId;
        }
    }
}
