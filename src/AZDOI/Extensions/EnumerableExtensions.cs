using System.Runtime.CompilerServices;
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




    public static async IAsyncEnumerable<TResult> ProcessResultsAsTheyCompleteAsync<T, TResult>(
        this IEnumerable<T> source, 
        Func<T, CancellationToken, Task<TResult>> processor,
        int maxDegreeOfParallelism,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
        )
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(maxDegreeOfParallelism, 1);

        using IEnumerator<T> enumerator = source.GetEnumerator();
        var tasks = new HashSet<Task<TResult>>();

        // Local function to enqueue tasks while respecting maxDegreeOfParallelism.
        void EnqueueTasks()
        {
            while (tasks.Count < maxDegreeOfParallelism && enumerator.MoveNext())
            {
                tasks.Add(processor(enumerator.Current, cancellationToken));
            }
        }

        // Initial population of tasks.
        EnqueueTasks();
        
        while (tasks.Count > 0)
        {
            // Wait for any task to complete.
            Task<TResult> completedTask = await Task.WhenAny(tasks);
            tasks.Remove(completedTask);
            
            // Await the completed task's result and yield it.
            yield return await completedTask;

            cancellationToken.ThrowIfCancellationRequested();

            // Enqueue additional tasks if there are any remaining items.
            EnqueueTasks();
        }
    }

    public static async IAsyncEnumerable<TResult> ProcessResultsAsTheyCompleteAsync<T, TResult>(
        this IAsyncEnumerable<T> source, 
        Func<T, CancellationToken, Task<TResult>> processor,
        int maxDegreeOfParallelism,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(maxDegreeOfParallelism, 1);

        await using IAsyncEnumerator<T> enumerator = source.GetAsyncEnumerator(cancellationToken);
        var tasks = new HashSet<Task<TResult>>();

        // Local function to enqueue tasks while respecting maxDegreeOfParallelism.
        async ValueTask EnqueueTasks()
        {
            while (tasks.Count < maxDegreeOfParallelism && await enumerator.MoveNextAsync())
            {
                tasks.Add(processor(enumerator.Current, cancellationToken));
            }
        }

        // Initial population of tasks.
        await EnqueueTasks();
        
        while (tasks.Count > 0)
        {
            // Wait for any task to complete.
            Task<TResult> completedTask = await Task.WhenAny(tasks);
            tasks.Remove(completedTask);
            
            // Await the completed task's result and yield it.
            yield return await completedTask;

            cancellationToken.ThrowIfCancellationRequested();

            // Enqueue additional tasks if there are any remaining items.
            await EnqueueTasks();
        }
    }
}
