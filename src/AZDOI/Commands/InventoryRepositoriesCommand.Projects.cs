using Cake.Common.IO;

namespace AZDOI.Commands;

public partial class InventoryRepositoriesCommand
{
    private async Task<int> ProcessProjects(InventoryRepositoriesContext context)
    {
        logger.LogInformation("Cleaning directory {TargetPath}...", context.OutputDirectory);
        cakeContext.CleanDirectory(context.OutputDirectory);
        logger.LogInformation("Done cleaning directory {TargetPath}.", context.OutputDirectory);

        var organization = new AzureDevOpsOrganization
        {
            Id = context.Settings.DevOpsOrg,
            Name = context.Settings.DevOpsOrg,
            Url = string.Empty,

            Children = (await context.InvokeDevOpsClient(
                (client, settings) => client.GetProjects(settings.DevOpsOrg, settings.IncludeProjects, settings.ExcludeProjects)
                )).OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase).ToArray()
        };

        foreach (var project in organization.Children)
        {
            var projectOutputDirectory = context.OutputDirectory.Combine(project.Name);

            var repositories = await ProcessRepositories(
            context with
            {
                OutputDirectory = context.OutputDirectory.Combine(project.Name)
            },
            project
            );

            await projectMarkdownService.WriteIndex
                (projectOutputDirectory,
                project with
                {
                    Children = repositories
                }
                );
            logger.LogInformation("Markdown index created for project: {ProjectName}", project.Name);
        }
        await organizationMarkdownService.WriteIndex(context.OutputDirectory, organization);

        logger.LogInformation("Done executing Inventory Repositories");
        return 0;
    }
}