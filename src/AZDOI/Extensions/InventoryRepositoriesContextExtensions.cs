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
}

