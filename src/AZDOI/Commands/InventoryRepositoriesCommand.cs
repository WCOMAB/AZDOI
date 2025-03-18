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
    ILogger<InventoryRepositoriesCommand> logger)
    : AsyncCommand<AZDOISettings>
{
    public override async Task<int> ExecuteAsync(CommandContext cmdContext, AZDOISettings settings)
    {
        try
        {
            var shouldProcessRepoReadme = (
                    settings.IncludeRepositoriesReadme,
                    settings.ExcludeRepositoriesReadme
                )
                .ToFilterPredicate();

            var orgOutputDirectory = settings.OutputPath.Combine(settings.DevOpsOrg);

            var context = new InventoryRepositoriesContext(
                settings,
                shouldProcessRepoReadme,
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
    }
}