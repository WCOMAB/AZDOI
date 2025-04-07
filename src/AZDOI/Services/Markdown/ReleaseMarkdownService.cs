namespace AZDOI.Services.Markdown;

public class ReleaseMarkdownService(ICakeContext cakeContext, TimeProvider timeProvider)
    : MarkdownServiceBase<AzureDevOpsRelease>(cakeContext, timeProvider)
{
    protected override async Task WriteIndex(FileTextWriter writer, AzureDevOpsRelease release)
    {
        await writer.WriteLineAsync(
            $$"""
            # {{release.Name}}
            
            ## Release Details

            """
        );

        await WriteTable(
            writer,
            [
                GetKeyValue(release.Id),
                GetKeyValue(release.Name),
                GetKeyValue(release.WebUrl),
                GetKeyValue(release.Path),
                GetKeyValue(release.Revision),
            ]
        );
    }
}
