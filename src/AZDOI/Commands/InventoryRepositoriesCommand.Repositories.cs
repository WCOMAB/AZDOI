namespace AZDOI.Commands;

public partial class InventoryRepositoriesCommand
{
    private async Task<AzureDevOpsRepository[]> ProcessRepositories(InventoryRepositoriesContext context, AzureDevOpsProject project)
    {
        using (logger.BeginScope(new { ProjectId = project.Id }))
        {
            logger.LogInformation("Project: {ProjectName}", project.Name);

            var repositoriesDirectory = context.OutputDirectory.Combine("Repositories");

            var repositories = await context.InvokeDevOpsClient((client, settings) =>
                client.GetRepositories(settings.DevOpsOrg, project.Id, settings.IncludeRepositories, settings.ExcludeRepositories));

            await repositoriesMarkdownService.WriteIndex(repositoriesDirectory, repositories);

            foreach (var repo in repositories)
            {
                await ProcessRepository(context, project, repo, repositoriesDirectory);
            }

            return repositories;
        }
    }

    private async Task ProcessRepository(InventoryRepositoriesContext context, AzureDevOpsProject project, AzureDevOpsRepository repo, DirectoryPath repositoriesDirectory)
    {
        using (logger.BeginScope(new { Repository = repo.Id }))
        {
            logger.LogInformation(" - Repository: {RepoName}", repo.Name);

            if (!context.ShouldProcessRepoReadme(repo.Name))
            {
                logger.LogInformation("Excluded repository: {RepoName}", repo.Name);
                return;
            }

            var readmeContent = string.Empty;
            var repoOutputDirectory = repositoriesDirectory.Combine(repo.Name);
            var (Exists, Length) = ((await context.InvokeDevOpsClient((client, settings) =>
                client.GetRepositoryReadme(settings.DevOpsOrg, project.Id, repo.Id)))
                ?.ReplaceLineEndings("\n")) is string c && c.Length > 0
                ? (true, (readmeContent = c).Length)
                : (false, 0);

            await repositoryMarkdownService.WriteIndex(
                repoOutputDirectory,
                repo with
                {
                    ReadmeContent = readmeContent,
                    Children = await context.InvokeDevOpsClient((client, settings) =>
                    client.GetBranchesAsync(settings.DevOpsOrg, project.Id, repo.Id)),
                    Tags = await context.InvokeDevOpsClient(
                            async (client, settings) =>
                                    await client.EnumerateTagsAsync(settings.DevOpsOrg, project.Id, repo.Id)
                                        .SelectAwait(async tag => tag with
                                        {
                                            Message = $"*{(await client.GetAnnotatedTagsAsync(settings.DevOpsOrg, project.Id, repo.Id, tag.ObjectId))
                                                        .FirstOrDefault()?.Message}*"
                                        }
                                        )
                                        .ToArrayAsync()
                    )
                }
            );

            logger.LogInformation("Markdown index created for repository: {RepoName}", repo.Name);

            logger.LogInformation(
                "README Exists: {Exists}, Length: {Length} characters",
                Exists,
                Length
            );
        }
    }
}