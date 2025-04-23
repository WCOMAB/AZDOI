using System.Runtime.CompilerServices;

namespace AZDOI.Services.Markdown;

public abstract partial class MarkdownServiceBase<TValue>
{
    protected static KeyValuePair<string, string> GetKeyValue(
        object? value,
        [CallerArgumentExpression(nameof(value))]
        string name = ""
    )
    {
        return new KeyValuePair<string, string>(
            name.Contains('.')
             ? name[(name.LastIndexOf('.') + 1)..]
             : name
            ,
            value?.ToString() is string str && System.Text.RegularExpressions.Regex.IsMatch(str, @"^https?://\S+$")
                ? $"[{str.Replace("https://", "").Replace("http://", "")}]({str})"
                : FormattableString.Invariant($"{value}")
            );
    }

    protected async Task WriteTable(
        FileTextWriter writer,
        KeyValuePair<string, string>[] keyValue,
        string keyHeader = "",
        string valueHeader = ""
        )
    {
        if (keyValue == null || keyValue.Length == 0)
            return;

        (int keyPadding, int valuePadding) = CalculatePadding(keyValue);

        await writer.WriteLineAsync(
            $"""
            | {keyHeader.PadRight(keyPadding)} | {valueHeader.PadRight(valuePadding)} |
            |-{new string('-', keyPadding)}-|-{new string('-', valuePadding)}-|
            """
        );

        foreach (var kvp in keyValue)
        {
            await writer.WriteLineAsync($"| {kvp.Key.PadRight(keyPadding)} | {kvp.Value.PadRight(valuePadding)} |");
        }
        await writer.WriteLineAsync();
    }

    private static (int keyPadding, int valuePadding) CalculatePadding(KeyValuePair<string, string>[] keyValue, int fixedPadding = 32)
    {
        if (keyValue == null || keyValue.Length == 0)
            return (fixedPadding, fixedPadding);

        int maxKeyLength = keyValue.Max(kvp => string.IsNullOrEmpty(kvp.Key) ? fixedPadding : kvp.Key.Length);
        int maxValueLength = keyValue.Max(kvp => string.IsNullOrEmpty(kvp.Value) ? fixedPadding : kvp.Value.Length);

        int GetPadding(int length) => (length + (fixedPadding - 1)) / fixedPadding * fixedPadding;

        int keyPadding = GetPadding(maxKeyLength);
        int valuePadding = GetPadding(maxValueLength);

        return (keyPadding, valuePadding);
    }
}
