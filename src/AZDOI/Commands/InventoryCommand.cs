using AZDOI.Commands.Settings;

namespace AZDOI.Commands;

public abstract partial class InventoryCommand<TSettings>(
    InventoryCommandServices services,
    ILogger logger
    ) : AsyncCommand<TSettings>
    where TSettings : AZDOISettings
{
    protected ICakeContext CakeContext { get; } = services.CakeContext;
    protected ILogger Logger { get; } = logger;

    public override async Task<int> ExecuteAsync(CommandContext cmdContext, TSettings settings)
    {
        services.StopwatchProvider.Start();

        var commandName = settings.ProjectChildTypes switch
        {
            AzureDevOpsProjectChildTypes.Repositories => nameof(AzureDevOpsProjectChildTypes.Repositories),
            AzureDevOpsProjectChildTypes.Pipelines => nameof(AzureDevOpsProjectChildTypes.Pipelines),
            AzureDevOpsProjectChildTypes.Repositories | AzureDevOpsProjectChildTypes.Pipelines =>
            string.Join(",", nameof(AzureDevOpsProjectChildTypes.Repositories), nameof(AzureDevOpsProjectChildTypes.Pipelines)),
            _ => "Unknown"
        };

        try
        {
            var orgOutputDirectory = settings.OutputPath.CombineEscapeUri(settings.DevOpsOrg);

            var context = new InventoryContext(
                settings,
                orgOutputDirectory,
                services.ClientHandler
            );

            Logger.LogInformation("Executing Inventory {commandName} Command...", commandName);
            return await ProcessProjectsPipeline(context);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failure during executing Inventory {commandName} Command.", commandName);
            return 1;
        }
        finally
        {
            services.StopwatchProvider.Stop();
            Logger.LogInformation("Processed inventory in {Elapsed}.", services.StopwatchProvider.Elapsed);
        }
    }
}