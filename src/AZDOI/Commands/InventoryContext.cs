using AZDOI.Commands.Settings;
using AZDOI.Services.AzureDevOps;

namespace AZDOI.Commands;

public record InventoryContext(
    AZDOISettings Settings,
    DirectoryPath OutputDirectory,
    AzureDevOpsClientHandler AzureDevOpsClientHandler
)
{
    public FilterHandler ShouldProcessRepoReadme { get; init; } = 
    (
        Settings.IncludeRepositoriesReadme,
        Settings.ExcludeRepositoriesReadme
    )
    .ToFilterPredicate();

    public async Task<TResult> InvokeDevOpsClient<TResult>(
        Func<AzureDevOpsClient, AZDOISettings, Task<TResult>> clientFunc)
    {
        using var client = await AzureDevOpsClientHandler(Settings);
        return await clientFunc(client, Settings);
    }
}
public delegate bool FilterHandler(string name);
