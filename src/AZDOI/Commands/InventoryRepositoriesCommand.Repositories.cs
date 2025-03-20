namespace AZDOI.Commands;

public partial class InventoryRepositoriesCommand
{
    private async IAsyncEnumerable<AzureDevOpsRepository> ProcessRepositories(InventoryRepositoriesContext context, AzureDevOpsProject project)
    {
        logger.LogInformation("Project: {ProjectName}", project.Name);

        var repositoriesDirectory = context.OutputDirectory.Combine("Repositories");

        var repositories = await context.InvokeDevOpsClient((client, settings) =>
            client.GetRepositories(settings.DevOpsOrg, project.Id, settings.IncludeRepositories, settings.ExcludeRepositories));

        await repositoriesMarkdownService.WriteIndex(repositoriesDirectory, repositories);

        foreach (var repo in repositories)
        {
            yield return await ProcessRepository(
                context with
                {
                    OutputDirectory = repositoriesDirectory,
                },
                project,
                repo
            );
        }
    }

    private async Task<AzureDevOpsRepository> ProcessRepository(InventoryRepositoriesContext context, AzureDevOpsProject project, AzureDevOpsRepository sourceRepo)
    {
        using (logger.BeginScope(new { Repository = sourceRepo.Id }))
        {
            logger.LogInformation("Project: {ProjectName} - Repository: {RepoName}", project.Name, sourceRepo.Name);

            var shouldProcessRepoReadme = context.ShouldProcessRepoReadme(sourceRepo.Name);

            var repo = await context.InvokeDevOpsClient(
                        async (client, settings) =>
                            sourceRepo with
                            {
                                ReadmeContent = shouldProcessRepoReadme
                                                    ? await client.GetRepositoryReadme(settings.DevOpsOrg, project.Id, sourceRepo.Id)
                                                    : "> ℹ️ Readme excluded",
                                Children = await client.GetBranchesAsync(settings.DevOpsOrg, project.Id, sourceRepo.Id),
                                Tags = await client.EnumerateTagsAsync(settings.DevOpsOrg, project.Id, sourceRepo.Id)
                                        .SelectAwait(
                                            async tag => tag with
                                                           {
                                                               Message = (await client.GetAnnotatedTagsAsync(settings.DevOpsOrg, project.Id, sourceRepo.Id, tag.ObjectId))
                                                                            .Select(aTag => $"*{aTag.Message}*")
                                                                            .FirstOrDefault()
                                                           }
                                        )
                                        .ToArrayAsync()
                            }
                    );

            await repositoryMarkdownService.WriteIndex(
                context.OutputDirectory.Combine(repo.Name),
                repo
            );

            logger.LogInformation("Project: {ProjectName} - Repository: {RepoName} - Markdown index created.", project.Name, repo.Name);

            if (shouldProcessRepoReadme)
            {

                var (Exists, Length) =  repo.ReadmeContent?.ReplaceLineEndings("\n").Length is int length
                                        &&
                                        length > 0
                    ? (true, length)
                    : (false, 0);

                logger.LogInformation(
                    "Project: {ProjectName} - Repository: {RepoName} - README Exists: {Exists}, Length: {Length} characters",
                    project.Name,
                    repo.Name,
                    Exists,
                    Length
                );
            }
            else
            {
                logger.LogInformation(
                    "Project: {ProjectName} - Repository: {RepoName} - README Excluded.",
                    project.Name,
                    repo.Name
                );
            }

            return repo;
        }
    }
}