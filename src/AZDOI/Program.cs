using AZDOI.Commands;

public partial class Program
{
    static partial void AddServices(IServiceCollection services)
    {
        services
            .AddCakeCore()
            .AddMarkdownServices()
            .AddSingleton(TimeProvider.System)
            .AddAzureDevOpsClient();
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
            }
        );
    }
}