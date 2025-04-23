namespace AZDOI.Services.Markdown;

public abstract partial class MarkdownServiceBase<TValue>
{
    protected async Task WriteChildren<TChildrenValue>(
    FileTextWriter writer,
    TChildrenValue[] children,
    string column,
    string? title = null,
    string description = "Description",
    Func<TChildrenValue, string>? urlSelector = null,
    int headingLevel = 2
)
    where TChildrenValue : IAzureDevOpsBase
    {
        await writer.WriteLineAsync($"{new string('#', headingLevel)} {title ?? column + "s"}");
        await writer.WriteLineAsync();

        if (children.Length == 0)
        {
            await writer.WriteLineAsync($"> ℹ️ No {(title ?? column + "s").ToLowerInvariant()} found.");
            return;
        }
        await WriteTable(
            writer,
            children
                .Select(child =>
                    new KeyValuePair<string, string>(
                        $"[{child.Name}](<{(urlSelector?.Invoke(child) ?? child.ChildUrl).PathEscapeUriString()}>)",
                        child.Description
                    )
                )
                .ToArray(),
            column,
            description
        );
    }
}
