namespace AZDOI.Tests.Extensions;

public static class MarkdownServiceTestExtensions
{
    public static async Task<string> TestWriteIndex<TValue>(this MarkdownServiceBase<TValue> service, TValue value)
    {
        return await service.WriteIndex("/", value) is FakeFile file
                        ? file.GetTextContent()
                        : throw new Exception("File is not a FakeFile");
    }
}
