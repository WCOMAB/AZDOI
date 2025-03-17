using Cake.Common.IO;

namespace AZDOI.Services.Markdown;

public abstract partial class MarkdownServiceBase<TValue>
{

    public virtual async Task<IFile> WriteIndex(DirectoryPath targetPath, TValue value)
    {
        using var writer = OpenIndexWrite(targetPath);
        await WriteFrontMatter(writer, value);
        await WriteIndex(writer, value);
        return writer.File;
    }

    protected virtual FileTextWriter OpenIndexWrite(
            DirectoryPath targetPath,
            string markDownFileName = "index.md"
            )
    {
        lock (cakeContext.FileSystem)
        {
            cakeContext.EnsureDirectoryExists(targetPath);
        }

        var file = cakeContext
                        .FileSystem
                        .GetFile(targetPath.CombineWithFilePath(markDownFileName));

        var writer = new FileTextWriter(
            file,
            Encoding.UTF8
        );

        return writer;
    }
}
