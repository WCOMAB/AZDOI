using AZDOI.Services.Markdown;

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
