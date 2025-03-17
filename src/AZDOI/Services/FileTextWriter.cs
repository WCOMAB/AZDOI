namespace AZDOI.Services;

public class FileTextWriter(
    IFile file,
    Encoding? encoding = null,
    FileMode fileMode = FileMode.Create,
    FileAccess fileAccess = FileAccess.Write,
    FileShare fileShare = FileShare.None
    ) : StreamWriter(
        file.Open(
            fileMode,
            fileAccess,
            fileShare
        ),
        encoding ?? Encoding.UTF8
    )
{
    public IFile File { get; } = file;
    public FilePath Path { get; } = file.Path;
}