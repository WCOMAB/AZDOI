namespace AZDOI.Extensions;

public static class StringExtensions
{
    public static string PathEscapeUriString(this string path)
        => path
            .TrimStart('/', '\\')
            .Aggregate(
                new StringBuilder(),
                (sb, c) =>
                    !char.IsAsciiLetterOrDigit(c)
                    && c != '/'
                    && c != '-'
                    && c != '_'
                    && c != '.'
                        ? sb.Append("0x").Append(Convert.ToHexString(Encoding.UTF8.GetBytes([c])))
                        : sb.Append(c),
                sb => sb.ToString()
            );

    public static string HtmlEncode(this string text)
        => WebUtility.HtmlEncode(text);
}
