using Cake.Common.IO;

namespace AZDOI.Commands;

public partial class InventoryCommand<TSettings>
{
    private async Task<int> ProcessProjectsPipeline(InventoryContext context)
    {
        Logger.LogInformation("Cleaning directory {TargetPath}...", context.OutputDirectory);
        services.CakeContext.CleanDirectory(context.OutputDirectory);
        Logger.LogInformation("Done cleaning directory {TargetPath}.", context.OutputDirectory);

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
                                using var _ = Logger.BeginScope(new { ProjectId = sourceProject.Id });

                                var projectOutputDirectory = context.OutputDirectory.Combine(sourceProject.Name);

                                var buildDirectory = projectOutputDirectory.Combine("Build");

                                var project = sourceProject with
                                {
                                    Children = await ProcessRepositories(
                                                context with
                                                {
                                                    OutputDirectory = projectOutputDirectory
                                                },
                                                sourceProject
                                    ),
                                    ChildTypes = context.Settings.ProjectChildTypes,
                                    Pipelines = await ProcessPipelines(
                                                context with
                                                {
                                                    OutputDirectory = buildDirectory,
                                                },
                                                sourceProject
                                    )
                                };

                                await services.ProjectMarkdownService.WriteIndex(projectOutputDirectory, project);

                                if (project.ChildTypes.HasFlag(AzureDevOpsProjectChildTypes.Pipelines))
                                {
                                    await services.BuildsMarkdownService.WriteIndex(buildDirectory, project);
                                }

                                Logger.LogInformation("Markdown index created for project: {ProjectName}", project.Name);

                                return project;
                            }
            )
            .OrderBy(_ => _.Name, StringComparer.OrdinalIgnoreCase)
            .ToArrayAsync()
        };

        await services.OrganizationMarkdownService.WriteIndex(context.OutputDirectory, organization);

        return 0;
    }
}