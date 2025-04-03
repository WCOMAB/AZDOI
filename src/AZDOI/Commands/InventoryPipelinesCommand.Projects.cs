using Cake.Common.IO;

namespace AZDOI.Commands;

public partial class InventoryPipelinesCommand
{
    private async Task<int> ProcessProjectsPipeline(InventoryRepositoriesContext context)
    {
        logger.LogInformation("Cleaning directory {TargetPath}...", context.OutputDirectory);
        cakeContext.CleanDirectory(context.OutputDirectory);
        logger.LogInformation("Done cleaning directory {TargetPath}.", context.OutputDirectory);

        var organization = new AzureDevOpsOrganization
        {
            Id = context.Settings.DevOpsOrg,
            Name = context.Settings.DevOpsOrg,
            Url = string.Empty,
            SkipOrgGraph = context.Settings.SkipOrgGraph,
            Children = await context.ForEachAsync(
                            await context.InvokeDevOpsClient(
                                (client, settings) => client.GetProjects(settings.DevOpsOrg, settings.IncludeProjects, settings.ExcludeProjects)
                            ),
                        async (sourceProject, ct) =>
                        {
                            using var _ = logger.BeginScope(new { ProjectId = sourceProject.Id });

                            var projectOutputDirectory = context.OutputDirectory.Combine(sourceProject.Name);

                            var buildDirectory = projectOutputDirectory.Combine("Build");

                            var project = sourceProject with
                            {
                                ChildTypes = context.Settings.ProjectChildTypes,
                                Pipelines = await ProcessPipelines(
                                                    context with
                                                    {
                                                        OutputDirectory = buildDirectory,
                                                    },
                                                    sourceProject
                                )
                            };

                            await projectMarkdownService.WriteIndex(projectOutputDirectory, project);

                            await buildsMarkdownService.WriteIndex(buildDirectory, project);

                            logger.LogInformation("Markdown index created for project: {ProjectName}", project.Name);

                            return project;
                        }
            )
                    .OrderBy(_ => _.Name, StringComparer.OrdinalIgnoreCase)
                    .ToArrayAsync()
        };

        await organizationMarkdownService.WriteIndex(context.OutputDirectory, organization);

        logger.LogInformation("Done executing Inventory Pipelines");
        return 0;
    }
}