public record AzureDevOpsRepositoryTag : IAzureDevOpsBase
{
    private readonly string _name = default!;
    public string ChildName { get; private set; } = default!;
    public required string ObjectId { get; init; }
    public required string Name
    {
        get => _name;
        init
        {
            _name = value;
            ChildName = _name.StartsWith("refs/tags/")
                ? _name["refs/tags/".Length..]
                : _name;
        }
    }
    public required string Url { get; init; }
    internal string? Message { get; init; }

    // Implicit interface implementation
    string IAzureDevOpsBase.Description => string.IsNullOrWhiteSpace(Message)
                                            ? ObjectId
                                            : $"{ObjectId} ({Message})";
    string IAzureDevOpsBase.Id => ObjectId;
    string IAzureDevOpsBase.ChildUrl => Url;
    string IAzureDevOpsBase.Name => ChildName;
}