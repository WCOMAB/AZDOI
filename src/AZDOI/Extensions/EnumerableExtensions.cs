using AZDOI.Commands;

namespace AZDOI.Extensions;

public static class EnumerableExtensions
{
    public static T[] FilterEnumerable<T>(
        this IEnumerable<T> source,
        IEnumerable<string>? includeNames,
        IEnumerable<string>? excludeNames,
        Func<T, string> nameSelector,
        StringComparer? comparer = null)
    {
        comparer ??= StringComparer.OrdinalIgnoreCase;
        var includeFilter = includeNames?.ToHashSet(comparer) ?? [];
        var excludeFilter = excludeNames?.ToHashSet(comparer) ?? [];

        bool includeFilterEmpty = includeFilter.Count == 0;
        bool excludeFilterEmpty = excludeFilter.Count == 0;

        return [
            ..
            source
                .Where(p => nameSelector(p) is { Length: >0 } name
                      &&
                      (includeFilterEmpty || includeFilter.Contains(name))
                      &&
                      (excludeFilterEmpty || !excludeFilter.Contains(name))
                )
        ];
    }

    public static FilterHandler ToFilterPredicate(
       this (
           IEnumerable<string>? includeNames,
           IEnumerable<string>? excludeNames
           ) filter,
        StringComparer? comparer = null
       )
    {
        comparer ??= StringComparer.OrdinalIgnoreCase;
        var includeFilter = filter.includeNames?.ToHashSet(comparer) ?? [];
        var excludeFilter = filter.excludeNames?.ToHashSet(comparer) ?? [];

        bool includeFilterEmpty = includeFilter.Count == 0;
        bool excludeFilterEmpty = excludeFilter.Count == 0;

        if (includeFilterEmpty && excludeFilterEmpty)
        {
            return _ => true;
        }

        return name => (includeFilterEmpty || includeFilter.Contains(name))
                      &&
                      (excludeFilterEmpty || !excludeFilter.Contains(name));
    }

    public static ILookup<string, string> FilterToLookup(
        this (
           IEnumerable<string>? includeNames,
           IEnumerable<string>? excludeNames
           ) filter
        )
     => (filter.includeNames ?? [])
            .Except(filter.excludeNames ?? [], StringComparer.OrdinalIgnoreCase)
            .ToLookup(
                key => key,
                value => value
            );
}
