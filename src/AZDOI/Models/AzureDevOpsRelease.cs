namespace AZDOI.Models;

public record AzureDevOpsRelease : IAzureDevOpsBase
{
    public required string Name { get; init; }

    public required int Id { get; init; }

    internal string WebUrl => Links.Web.Href;

    public required int Revision {  get; init; }

    public required string Path { get; init; }

    public string? Description { get; init; }

    [JsonPropertyName("_links")]
    public required AzureDevOpsLinks Links { get; init; }

    string IAzureDevOpsBase.Description => Description ?? Name;
    string IAzureDevOpsBase.Id => FormattableString.Invariant($"{Id}");
    string IAzureDevOpsBase.Url => WebUrl;
}