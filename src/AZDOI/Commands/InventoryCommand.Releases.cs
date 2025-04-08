namespace AZDOI.Commands;

public partial class InventoryCommand<TSettings>
{
    private async Task<AzureDevOpsRelease[]> ProcessReleases(InventoryContext context, AzureDevOpsProject project)
    {
        if (!context.Settings.ProjectChildTypes.HasFlag(AzureDevOpsProjectChildTypes.Pipelines))
        {
            return [];
        }

        Logger.LogInformation("Project: {ProjectName}", project.Name);

        var releasesDirectory = context.OutputDirectory.Combine("Releases");

        var releases =
            await context.InvokeDevOpsClient(
                async (client, settings) =>
                    await context.ForEachAsync(
                        await client.GetReleases(settings.DevOpsOrg, project.Id, settings.IncludeReleases, settings.ExcludeReleases),
                        (release, ct) => ProcessRelease(
                                            context with
                                            {
                                                OutputDirectory = releasesDirectory,
                                            },
                                            project,
                                            release
                        )
                    )
                    .OrderBy(_ => _.Name, StringComparer.OrdinalIgnoreCase)
                    .ToArrayAsync()
            );
        await services.ReleasesMarkdownService.WriteIndex(releasesDirectory, releases);

        return releases;
    }

    private async Task<AzureDevOpsRelease> ProcessRelease(InventoryContext context, AzureDevOpsProject project, AzureDevOpsRelease sourceRelease)
    {
        using (Logger.BeginScope(new { Release = sourceRelease.Id }))
        {
            Logger.LogInformation("Project: {ProjectName} - Release: {ReleaseName}", project.Name, sourceRelease.Name);

            await services.ReleaseMarkdownService.WriteIndex(
                context.OutputDirectory.CombineEscapeUri(sourceRelease.Name),
                sourceRelease
            );

            Logger.LogInformation("Project: {ProjectName} - Release: {ReleaseName} - Markdown index created.", project.Name, sourceRelease.Name);

            Logger.LogInformation(
                "Project: {ProjectName} - Release: {ReleaseName}.",
                project.Name,
                sourceRelease.Name
            );
        }
        return sourceRelease;
    }
}