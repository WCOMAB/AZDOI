using AZDOI.Commands.Settings;
using AZDOI.Services.Markdown;

namespace AZDOI.Commands;

public partial class InventoryPipelinesCommand(
    ICakeContext cakeContext,
    AzureDevOpsClientHandler clientHandler,
    OrganizationMarkdownService organizationMarkdownService,
    ProjectMarkdownService projectMarkdownService,
    PipelineMarkdownService pipelineMarkdownService,
    PipelinesMarkdownService pipelinesMarkdownService,
    BuildsMarkdownService buildsMarkdownService,
    StopwatchProvider stopwatchProvider,
    ILogger<InventoryPipelinesCommand> logger)
    : AsyncCommand<AZDOIPipelinesSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext cmdContext, AZDOIPipelinesSettings settings)
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

            logger.LogInformation("Executing Inventory Pipelines Command...");
            return await ProcessProjectsPipeline(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failure during executing Inventory Pipelines Command.");
            return 1;
        }
        finally
        {
            stopwatchProvider.Stop();
            logger.LogInformation("Processed inventory in {Elapsed}.", stopwatchProvider.Elapsed);
        }
    }
}