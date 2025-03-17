namespace AZDOI.Models;

public record AzureDevOpsResponse<T>
{
    public required T[] Value { get; init; }
}
