using System.Runtime.CompilerServices;
using AZDOI.Commands;
using AZDOI.Commands.Settings;
using AZDOI.Services.AzureDevOps;

namespace AZDOI.Extensions;

public static class InventoryContextExtensions
{
    public static async Task<TResult> InvokeDevOpsClient<TResult>(
        this InventoryContext context,
        Func<AzureDevOpsClient, AZDOISettings, Task<TResult>> clientFunc)
    {
        using var client = await context.AzureDevOpsClientHandler(context.Settings);
        return await clientFunc(client, context.Settings);
    }

    static readonly int maxDegreeOfParallelism = Environment.ProcessorCount;

    public static async IAsyncEnumerable<TSource> ForEachAsync<TSource>(
        this InventoryContext context,
        IEnumerable<TSource> source,
        Func<TSource, CancellationToken, Task<TSource>> body,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
        )
    {
        if (context.Settings.RunInParallel)
        {
            await foreach (var result in source.ProcessResultsAsTheyCompleteAsync(body, maxDegreeOfParallelism, cancellationToken))
            {
                yield return result;
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }
        else
        {
            foreach (var v in source)
            {
                
                yield return await body(v, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }
    }

    public static async IAsyncEnumerable<TSource> ForEachAsync<TSource>(
        this InventoryContext context,
        IAsyncEnumerable<TSource> source,
        Func<TSource, CancellationToken, Task<TSource>> body,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
        )
    {
        if (context.Settings.RunInParallel)
        {
            await foreach (var result in source.ProcessResultsAsTheyCompleteAsync(body, maxDegreeOfParallelism, cancellationToken))
            {
                yield return result;
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }
        else
        {
            await foreach (var v in source)
            {
                yield return await body(v, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }
    }
}