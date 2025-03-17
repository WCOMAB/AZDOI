namespace AZDOI.Models;

public record AzureDevOpsRepository : AzureDevOpsBase<AzureDevOpsRepositoryBranch>
{
    public required long Size { get; init; }
    public required string RemoteUrl { get; init; }
    public required string WebUrl { get; init; }
    internal string? ReadmeContent { get; init; }
    public string? DefaultBranch { get; init; }
    internal AzureDevOpsRepositoryTag[] Tags { get; init; } = [];
}
