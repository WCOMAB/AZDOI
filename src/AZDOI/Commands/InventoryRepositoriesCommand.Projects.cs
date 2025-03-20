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
            Children = await context.ForEachAsync(
                        (
                            await context.InvokeDevOpsClient(
                                (client, settings) => client.GetProjects(settings.DevOpsOrg, settings.IncludeProjects, settings.ExcludeProjects)
                            )
                        )
                        .OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase),
                        async (sourceProject, ct) =>
                        {
                            using var _ = logger.BeginScope(new { ProjectId = sourceProject.Id });
                            
                            var projectOutputDirectory = context.OutputDirectory.Combine(sourceProject.Name);

                            var project = sourceProject with
                            {
                                Children = await ProcessRepositories(
                                                    context with
                                                    {
                                                        OutputDirectory = projectOutputDirectory
                                                    },
                                                    sourceProject
                                                )
                                                .ToArrayAsync(ct)
                            };

                            await projectMarkdownService.WriteIndex(
                                projectOutputDirectory,
                                project
                                );

                            logger.LogInformation("Markdown index created for project: {ProjectName}", project.Name);

                            return project;
                        }
                    )
                    .ToArrayAsync()
        };

        await organizationMarkdownService.WriteIndex(context.OutputDirectory, organization);

        logger.LogInformation("Done executing Inventory Repositories");
        return 0;
    }
}