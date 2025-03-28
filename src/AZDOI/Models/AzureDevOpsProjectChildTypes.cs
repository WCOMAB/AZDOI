namespace AZDOI.Models;

[Flags]
public enum AzureDevOpsProjectChildTypes:byte
{
    None = 0,
    Repositories = 1,
    Pipelines = 2,
    All = 255
}
