namespace AZDOI.Services.Markdown;

public class RepositoryMarkdownService(ICakeContext cakeContext, TimeProvider timeProvider)
    : MarkdownServiceBase<AzureDevOpsRepository>(cakeContext, timeProvider)
{
    protected override async Task WriteIndex(FileTextWriter writer, AzureDevOpsRepository repository)
    {
        await writer.WriteLineAsync(
            $$"""
            # {{repository.Name}}

            ## Repository Details

            """
        );

        var defaultBranchLink = repository.WebUrl.BuildBranchMarkdownLink(repository.DefaultBranch ?? "");

        await WriteTable(
            writer,
            [
                GetKeyValue(repository.Name),
                GetKeyValue(repository.Id),
                GetKeyValue(repository.Size.FormatBytes(), "Size"),
                GetKeyValue(repository.RemoteUrl),
                GetKeyValue(repository.WebUrl),
                GetKeyValue(defaultBranchLink, "DefaultBranch")
            ]
            );

        await WriteChildren(
            writer,
            repository.Children,
            "Branch",
            "Branches",
            "ObjectId",
            branch => repository.WebUrl.BuildBranchUrl(branch.ChildName)
        );

        await WriteChildren(
            writer,
            repository.Tags,
            "Tag",
            "Tags",
            "ObjectId",
            tag => repository.WebUrl.BuildTagUrl(tag.ChildName)
        );

        await writer.WriteLineAsync(
           $$"""
            ## README

            {{repository.ReadmeContent.IncreaseMarkdownHeaders()}}
            """
       );
    }
}