namespace AZDOI.Models;

public record AzureDevOpsRepositoryBranch : IAzureDevOpsBase
{
    private readonly string name = default!;
    public string ChildName { get; private set; } = default!;
    public required string ObjectId { get; init; }
    public required string Name
    {
        get => name;
        init
        {
            name = value;
            ChildName = name.CleanBranchName();
        }
    }
    public required string Url { get; init; }

    string IAzureDevOpsBase.Description => ObjectId;
    string IAzureDevOpsBase.Id => ObjectId;
    string IAzureDevOpsBase.ChildUrl => Url;
    string IAzureDevOpsBase.Name => ChildName;
}
