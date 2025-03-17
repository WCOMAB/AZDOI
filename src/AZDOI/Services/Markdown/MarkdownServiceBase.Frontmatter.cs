namespace AZDOI.Services.Markdown;

public abstract partial class MarkdownServiceBase<TValue>
{
    protected virtual async Task WriteFrontMatter(FileTextWriter writer, TValue value)
    {
        await writer.WriteLineAsync("---");
        if (value is AzureDevOpsBase azureDevOps)
        {
            await writer.WriteLineAsync($"title: {azureDevOps.Name}");
            await writer.WriteLineAsync($"summary: {(string.IsNullOrEmpty(azureDevOps.Description) ? azureDevOps.Name : azureDevOps.Description)}");
        }
        await writer.WriteLineAsync($"modifiedby: AZDOI");
        await writer.WriteLineAsync($"modified: {timeProvider.GetUtcNow():yyyy-MM-dd HH:mm}");
        await writer.WriteLineAsync("---");
    }
}
