using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;

namespace AZDOI.Services.Markdown;

public abstract partial class MarkdownServiceBase<TValue>
{
    protected virtual string? Title { get; }
    protected virtual string? Summary { get; }

    private static readonly ISerializer YamlSeralizer = new SerializerBuilder()
                                                            .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                                            .Build();

    protected virtual async Task WriteFrontMatter(FileTextWriter writer, TValue value)
    {
        await writer.WriteLineAsync("---");
        var azureDevOps = value as IAzureDevOpsBase;
        var title = Title ?? azureDevOps?.Name;
        var summary = Summary ?? (
            string.IsNullOrEmpty(azureDevOps?.Description) ? azureDevOps?.Name : azureDevOps?.Description
            );


        if (!string.IsNullOrWhiteSpace(title) )
        {
            YamlSeralizer.Serialize(writer, new
            {
                title
            });
        }

        if (!string.IsNullOrWhiteSpace(summary))
        {
            YamlSeralizer.Serialize(writer, new
            {
                summary
            });
        }

        await writer.WriteLineAsync($"modifiedby: AZDOI");
        await writer.WriteLineAsync($"modified: {timeProvider.GetUtcNow():yyyy-MM-dd HH:mm}");
        await writer.WriteLineAsync("---");
    }
}
