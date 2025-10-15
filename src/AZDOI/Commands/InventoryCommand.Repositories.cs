namespace AZDOI.Commands;

public partial class InventoryCommand<TSettings>
{
    private async Task<AzureDevOpsRepository[]> ProcessRepositories(InventoryContext context, AzureDevOpsProject project)
    {
        if (!context.Settings.ProjectChildTypes.HasFlag(AzureDevOpsProjectChildTypes.Repositories))
        {
            return [];
        }

        Logger.LogInformation("Project: {ProjectName}", project.Name);

        var repositoriesDirectory = context.OutputDirectory.Combine("Repositories");

        var repositories = await context.InvokeDevOpsClient(
            async (client, settings) => await context.ForEachAsync(
                    await client.GetRepositories(settings.DevOpsOrg, project.Id, settings.IncludeRepositories, settings.ExcludeRepositories),

                    (repo, ct) => ProcessRepository(
                        context with
                        {
                            OutputDirectory = repositoriesDirectory,
                        },
                        project,
                        repo
                    )
            )
            .OrderBy(_ => _.Name, StringComparer.OrdinalIgnoreCase)
            .ToArrayAsync()
        );

        await services.RepositoriesMarkdownService.WriteIndex(repositoriesDirectory, repositories);

        return repositories;
    }

    private async Task<AzureDevOpsRepository> ProcessRepository(InventoryContext context, AzureDevOpsProject project, AzureDevOpsRepository sourceRepo)
    {
        using (Logger.BeginScope(new { Repository = sourceRepo.Id }))
        {
            Logger.LogInformation("Project: {ProjectName} - Repository: {RepoName}", project.Name, sourceRepo.Name);

            var shouldProcessRepoReadme = context.ShouldProcessRepoReadme(sourceRepo.Name);

            var repo = await context.InvokeDevOpsClient(
                async (client, settings) =>
                    sourceRepo with
                    {
                        ReadmeContent = shouldProcessRepoReadme
                                            ? await client.GetRepositoryReadme(settings.DevOpsOrg, project.Id, sourceRepo.Id)
                                            : "> ℹ️ Readme excluded",

                        Children = await client
                                    .GetBranchesAsync(settings.DevOpsOrg, project.Id, sourceRepo.Id)
                                    .OrderBy(_ => !StringComparer.OrdinalIgnoreCase.Equals(sourceRepo.DefaultBranch, _.Name))
                                    .ThenBy(_ => _.Name, StringComparer.OrdinalIgnoreCase)
                                    .ToArrayAsync(),

                        Tags = await Task.Run(async () =>
                        {
                            var tagsList = new List<AzureDevOpsRepositoryTag>();
                            await foreach (var tag in client.GetTagsAsync(settings.DevOpsOrg, project.Id, sourceRepo.Id))
                            {
                                var annotatedTag = await client.GetAnnotatedTagsAsync(
                                    settings.DevOpsOrg,
                                    project.Id,
                                    sourceRepo.Id,
                                    tag.ObjectId
                                );

                                tagsList.Add(tag with { Message = annotatedTag?.Message });
                            }


                            return tagsList
                                .OrderBy(tag => tag.Name, StringComparer.OrdinalIgnoreCase)
                                .ToArray();
                        })
                    }
            );

            await services.RepositoryMarkdownService.WriteIndex(
                context.OutputDirectory.CombineEscapeUri(repo.Name),
                repo
            );

            Logger.LogInformation("Project: {ProjectName} - Repository: {RepoName} - Markdown index created.", project.Name, repo.Name);

            if (shouldProcessRepoReadme)
            {
                var (Exists, Length) = repo.ReadmeContent?.ReplaceLineEndings("\n").Length is int length
                                        &&
                                        length > 0
                                        ? (true, length)
                                        : (false, 0);

                Logger.LogInformation(
                    "Project: {ProjectName} - Repository: {RepoName} - README Exists: {Exists}, Length: {Length} characters",
                    project.Name,
                    repo.Name,
                    Exists,
                    Length
                );
            }
            else
            {
                Logger.LogInformation(
                    "Project: {ProjectName} - Repository: {RepoName} - README Excluded.",
                    project.Name,
                    repo.Name
                );
            }
            return repo;
        }
    }
}