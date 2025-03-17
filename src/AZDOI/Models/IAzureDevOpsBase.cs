namespace AZDOI.Models;

public interface IAzureDevOpsBase
{
    string Description { get; }
    string Id { get; }
    string Name { get; }
    string Url { get; }
    string ChildUrl => Name;
}