namespace AZDOI.Services.Markdown;

public abstract partial class MarkdownServiceBase<TValue>(ICakeContext cakeContext, TimeProvider timeProvider)
{
    protected abstract Task WriteIndex(FileTextWriter writer, TValue value);
}
