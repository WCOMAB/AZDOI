using AZDOI.Commands;
using AZDOI.Commands.Settings;
using AZDOI.Services.AzureDevOps;

namespace AZDOI.Extensions;

public static class InventoryRepositoriesContextExtensions
{
    public static async Task<TResult> InvokeDevOpsClient<TResult>(
        this InventoryRepositoriesContext context,
        Func<AzureDevOpsClient, AZDOISettings, Task<TResult>> clientFunc)
    {
        using var client = await context.AzureDevOpsClientHandler(context.Settings);
        return await clientFunc(client, context.Settings);
    }
    public static async Task ForEachAsync<TSource>(
      this InventoryRepositoriesContext context,
      IEnumerable<TSource> source,
      Func<TSource, CancellationToken, ValueTask> body)
    {
        if (context.Settings.RunInParallel)
        {
            await Parallel.ForEachAsync(source, body);
        }
        else
        {
            using var cts = new CancellationTokenSource();
            var ct = cts.Token;
            foreach (var v in source)
            {
                await body(v, ct);
                if (ct.IsCancellationRequested)
                {
                    return;
                }
            }
            return;
        }
    }
}