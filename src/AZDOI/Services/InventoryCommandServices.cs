using AZDOI.Services.Markdown.Builds;
using AZDOI.Services.Markdown.Builds.MarkdownReleases;
using AZDOI.Services.Markdown.Builds.Pipelines;
using AZDOI.Services.Markdown.Organization;
using AZDOI.Services.Markdown.Projects;
using AZDOI.Services.Markdown.Repositories;

namespace AZDOI.Services;
public record InventoryCommandServices(
    ICakeContext CakeContext,
    AzureDevOpsClientHandler ClientHandler,
    OrganizationMarkdownService OrganizationMarkdownService,
    ProjectMarkdownService ProjectMarkdownService,
    RepositoriesMarkdownService RepositoriesMarkdownService,
    RepositoryMarkdownService RepositoryMarkdownService,
    PipelineMarkdownService PipelineMarkdownService,
    PipelinesMarkdownService PipelinesMarkdownService,
    BuildsMarkdownService BuildsMarkdownService,
    StopwatchProvider StopwatchProvider,
    ReleaseMarkdownService ReleaseMarkdownService,
    ReleasesMarkdownService ReleasesMarkdownService
    );
