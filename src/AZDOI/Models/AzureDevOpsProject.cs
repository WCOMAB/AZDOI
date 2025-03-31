namespace AZDOI.Models;

public record AzureDevOpsProject : AzureDevOpsBase<AzureDevOpsRepository>
{
    internal AzureDevOpsProjectChildTypes ChildTypes { get; init; } = AzureDevOpsProjectChildTypes.Repositories;

    internal AzureDevOpsPipeline[] Pipelines { get; init; } = [];
}
