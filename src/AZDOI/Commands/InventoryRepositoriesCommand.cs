using AZDOI.Commands.Settings;
using AZDOI.Services.Markdown;

namespace AZDOI.Commands;

public partial class InventoryRepositoriesCommand(
    ICakeContext cakeContext,
    AzureDevOpsClientHandler clientHandler,
    OrganizationMarkdownService organizationMarkdownService,
    ProjectMarkdownService projectMarkdownService,
    RepositoriesMarkdownService repositoriesMarkdownService,
    RepositoryMarkdownService repositoryMarkdownService,
    StopwatchProvider stopwatchProvider,
    ILogger<InventoryRepositoriesCommand> logger)
    : AsyncCommand<AZDOISettings>
{
    public override async Task<int> ExecuteAsync(CommandContext cmdContext, AZDOISettings settings)
    {
        stopwatchProvider.Start();

        try
        {
            var orgOutputDirectory = settings.OutputPath.Combine(settings.DevOpsOrg);

            var context = new InventoryRepositoriesContext(
                settings,
                orgOutputDirectory,
                clientHandler
            );

            logger.LogInformation("Executing Inventory Repositories Command...");
            return await ProcessProjects(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failure during executing Inventory Repositories Command.");
            return 1;
        }
        finally
        {
            stopwatchProvider.Stop();
            logger.LogInformation("Processed inventory in {Elapsed}.", stopwatchProvider.Elapsed);
        }
    }
}