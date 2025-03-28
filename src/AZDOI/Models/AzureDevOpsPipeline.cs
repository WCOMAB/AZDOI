namespace AZDOI.Models;

public record AzureDevOpsPipeline : IAzureDevOpsBase
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    internal string WebUrl => Links.Web.Href;

    [JsonPropertyName("_links")]
    public required AzureDevOpsLinks Links { get; init; }

    string IAzureDevOpsBase.Description => Name;
    string IAzureDevOpsBase.Id => FormattableString.Invariant($"{Id}");
    string IAzureDevOpsBase.Url => WebUrl;
}


public record AzureDevOpsLinks(
        [property: JsonPropertyName("self")]
        AzureDevOpsLink Self,
        [property: JsonPropertyName("web")]
        AzureDevOpsLink Web
       );

public record AzureDevOpsLink(
    [property: JsonPropertyName("href")]
    string Href
    );
