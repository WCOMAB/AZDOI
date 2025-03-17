namespace AZDOI.Services.Markdown;

public abstract partial class MarkdownServiceBase<TValue>
{
    protected async Task WriteChildren<TChildrenValue>(
    FileTextWriter writer,
    TChildrenValue[] children,
    string column,
    string? title = null,
    string description = "Description",
    Func<TChildrenValue, string>? urlSelector = null
)
    where TChildrenValue : IAzureDevOpsBase
    {
        await writer.WriteLineAsync($"## {title ?? column + "s"}");
        await writer.WriteLineAsync();
        await WriteTable(
            writer,
            children
                .Select(child =>
                    new KeyValuePair<string, string>(
                        $"[{child.Name}]({urlSelector?.Invoke(child) ?? child.ChildUrl})",
                        child.Description
                    )
                )
                .ToArray(),
            column,
            description
        );
    }
}
