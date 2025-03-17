using System.Text.RegularExpressions;

namespace AZDOI.Extensions;

public static partial class MarkdownExtensions
{
    [GeneratedRegex(@"^(#+)(.*)$", RegexOptions.Multiline)]
    private static partial Regex HeaderRegex();

    public static string IncreaseMarkdownHeaders(this string? markdown, int increment = 2)
    {
        if (string.IsNullOrEmpty(markdown))
        {
            return string.Empty;
        }

        return HeaderRegex().Replace(
            markdown,
            m =>
            {
                int originalHeaderLevel = m.Groups[1].Value.Length;
                string newHeader = new string('#', originalHeaderLevel + increment);
                return newHeader + m.Groups[2].Value;
            }
        );
    }

    public static string CleanBranchName(this string branchName)
    {
        return branchName.StartsWith("refs/heads/")
            ? branchName["refs/heads/".Length..]
            : branchName;
    }

    public static string CleanTagName(this string tagName)
    {
        return tagName.StartsWith("refs/tags/")
            ? tagName["refs/tags/".Length..]
            : tagName;
    }

    public static string BuildTagUrl(this string repoWebUrl, string tagName)
    {
        string cleanName = tagName.CleanTagName();
        return $"{repoWebUrl}?version=GT{Uri.EscapeDataString(cleanName)}";
    }

    public static string BuildBranchUrl(this string repoWebUrl, string branchName)
    {
        string cleanName = branchName.CleanBranchName();
        return $"{repoWebUrl}?version=GB{Uri.EscapeDataString(cleanName)}";
    }

    public static string BuildBranchMarkdownLink(this string repoWebUrl, string branchName)
    {
        string cleanName = branchName.CleanBranchName();
        string url = repoWebUrl.BuildBranchUrl(branchName);
        return $"[{cleanName}]({url})";
    }

    public static string FormatBytes(this long bytes)
    {
        string[] units = { "bytes", "KB", "MB", "GB", "TB", "PB" };

        double value = bytes;
        int unitIndex = 0;

        while (value >= 1024 && unitIndex < units.Length - 1)
        {
            value /= 1024;
            unitIndex++;
        }

        double flooredValue = Math.Floor(value * 10) / 10;

        return $"{flooredValue.ToString("0.0", CultureInfo.InvariantCulture)} {units[unitIndex]}";
    }
}