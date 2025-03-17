using AZDOI.Commands.Settings;
using AZDOI.Services.Markdown;
using Cake.Common.IO;

namespace AZDOI.Commands;

public class InventoryRepositoriesCommand(
    ICakeContext cakeContext,
    AzureDevOpsClientHandler clientHandler,
    OrganizationMarkdownService organizationMarkdownService,
    ProjectMarkdownService projectMarkdownService,
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

            Children = await context.InvokeDevOpsClient(
                (client, settings) => client.GetProjects(settings.DevOpsOrg, settings.IncludeProjects, settings.ExcludeProjects)
                )
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

    private async Task<AzureDevOpsRepository[]> ProcessRepositories(InventoryRepositoriesContext context, AzureDevOpsProject project)
    {
        using (logger.BeginScope(new { ProjectId = project.Id }))
        {
            logger.LogInformation("Project: {ProjectName}", project.Name);

            var repositories = await ProcessRepository(context, project);
            return repositories;
        }
    }

    private async Task<AzureDevOpsRepository[]> ProcessRepository(InventoryRepositoriesContext context, AzureDevOpsProject project)
    {
        var repositoriesDirectory = context.OutputDirectory.Combine("Repositories");
        cakeContext.EnsureDirectoryExists(repositoriesDirectory);

        var repositories = await context.InvokeDevOpsClient((client, settings) =>
            client.GetRepositories(settings.DevOpsOrg, project.Id, settings.IncludeRepositories, settings.ExcludeRepositories));

        foreach (var repo in repositories)
        {
            using (logger.BeginScope(new { Repository = repo.Id }))
            {
                logger.LogInformation(" - Repository: {RepoName}", repo.Name);

                if (!context.ShouldProcessRepoReadme(repo.Name))
                {
                    logger.LogInformation("Excluded repository: {RepoName}", repo.Name);
                    continue;
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
                                                Message = (await client.GetAnnotatedTagsAsync(settings.DevOpsOrg, project.Id, repo.Id, tag.ObjectId))
                                                            .FirstOrDefault()?.Message
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
        return repositories;
    }
}