using AZDOI.Commands;

public partial class Program
{
    static partial void AddServices(IServiceCollection services)
    {
        services
            .AddCakeCore()
            .AddMarkdownServices()
            .AddSingleton(TimeProvider.System)
            .AddAzureDevOpsClient()
            .AddSingleton<StopwatchProvider, SystemStopwatch>();
    }
    static partial void ConfigureApp(AppServiceConfig appServiceConfig)
    {
        appServiceConfig.SetApplicationName("AZDOI");
        appServiceConfig.AddBranch(
            "inventory",
            branch =>
            {
                branch.AddCommand<InventoryRepositoriesCommand>("repositories")
                                    .WithDescription("Example get repositories command.")
                                    .WithExample(["inventory", "repositories"]);

                branch.AddCommand<InventoryPipelinesCommand>("pipelines")
                                    .WithDescription("Example get pipelines command.")
                                    .WithExample(["inventory", "pipelines"]);
            }
        );
    }
}