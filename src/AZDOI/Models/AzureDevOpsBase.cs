namespace AZDOI.Models;

public record AzureDevOpsBase : IAzureDevOpsBase
{
    
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string Description { get; init; } = string.Empty;
    public required string Url { get; init; }
}

public record AzureDevOpsBase<TChild> : AzureDevOpsBase
    where TChild : IAzureDevOpsBase
{
    internal TChild[] Children { get; init; } = [];
}