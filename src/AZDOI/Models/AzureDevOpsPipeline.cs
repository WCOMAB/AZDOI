namespace AZDOI.Models;

public record AzureDevOpsPipeline : IAzureDevOpsBase
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Url { get; init; }

    string IAzureDevOpsBase.Description => Name;
    string IAzureDevOpsBase.Id => Id.ToString();
}