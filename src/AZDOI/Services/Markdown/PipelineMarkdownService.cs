namespace AZDOI.Services.Markdown;

public class PipelineMarkdownService(ICakeContext cakeContext, TimeProvider timeProvider)
    : MarkdownServiceBase<AzureDevOpsPipeline>(cakeContext, timeProvider)
{
    protected override async Task WriteIndex(FileTextWriter writer, AzureDevOpsPipeline pipeline)
    {
        await writer.WriteLineAsync(
            $$"""
            # {{pipeline.Name}}
            
            ## Pipeline Details

            """
        );

        await WriteTable(
            writer,
            [
                GetKeyValue(pipeline.Id),
                GetKeyValue(pipeline.Name),
                GetKeyValue(pipeline.WebUrl),
                GetKeyValue(pipeline.Folder),
                GetKeyValue(pipeline.Revision),
            ]
        );
    }
}