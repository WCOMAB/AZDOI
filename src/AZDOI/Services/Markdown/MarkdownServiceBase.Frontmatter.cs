namespace AZDOI.Services.Markdown;

public abstract partial class MarkdownServiceBase<TValue>
{
    protected virtual string? Title { get; }
    protected virtual string? Summary { get; }


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
            await writer.WriteLineAsync($"title: {title}");
        }
        if (!string.IsNullOrWhiteSpace(summary))
        {
            await writer.WriteLineAsync($"summary: {summary}");
        }

        await writer.WriteLineAsync($"modifiedby: AZDOI");
        await writer.WriteLineAsync($"modified: {timeProvider.GetUtcNow():yyyy-MM-dd HH:mm}");
        await writer.WriteLineAsync("---");
    }
}
