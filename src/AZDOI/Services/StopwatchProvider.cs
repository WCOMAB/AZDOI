using System.Diagnostics;

namespace AZDOI.Services;

public abstract class StopwatchProvider
{
    public abstract TimeSpan Elapsed { get; }
    public abstract bool IsRunning { get; }
    public abstract bool IsHighResolution { get; }
    public abstract void Start();
    public abstract void Stop();
}

public class SystemStopwatch : StopwatchProvider
{
    private Stopwatch? _stopwatch;

    public override TimeSpan Elapsed => _stopwatch?.Elapsed ?? TimeSpan.Zero;
    public override bool IsRunning => _stopwatch?.IsRunning ?? false;
    public override bool IsHighResolution => Stopwatch.IsHighResolution;

    public override void Start()
    {
        _stopwatch ??= Stopwatch.StartNew();
    }

    public override void Stop()
    {
        _stopwatch?.Stop();
    }
}