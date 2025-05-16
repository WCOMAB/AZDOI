using AZDOI.Services;
using Microsoft.Extensions.Time.Testing;

namespace AZDOI.Tests.Services;

public class TestMarkdownService(ICakeContext cakeContext, FakeTimeProvider fakeTimeProvider)
    : MarkdownServiceBase<string>(cakeContext, fakeTimeProvider)
{
    protected override async Task WriteIndex(FileTextWriter writer, string value)
    {
        await writer.WriteAsync(value);
    }
}
