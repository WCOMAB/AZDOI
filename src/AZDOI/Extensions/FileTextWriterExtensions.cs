namespace AZDOI.Extensions;
public static class FileTextWriterExtensions
{
    public static async Task WriteClickNode(this FileTextWriter writer, string nodeId, string relativeUrl, string description)
    {
        var escapedUrl = relativeUrl.PathEscapeUriString();
        var line = $"            click {nodeId} href \"{escapedUrl}\" {JsonSerializer.Serialize(description)}";
        await writer.WriteLineAsync(line);
    }
}