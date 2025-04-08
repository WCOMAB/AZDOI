namespace AZDOI.Extensions;

public static class DirectoryPathExtensions
{
    public static DirectoryPath CombineEscapeUri(this DirectoryPath path, string childPath)
    {
        return path.Combine(childPath.PathEscapeUriString());
    }
}
