using AZDOI.Services;

namespace AZDOI.Tests.Services;

public class FakeStopwatch(TimeProvider timeProvider) : StopwatchProvider
{
    private long? _started;
    private bool _running;
    private TimeSpan? _stopped;

    public override TimeSpan Elapsed => _stopped ?? timeProvider.GetElapsedTime(_started ?? 0);
    public override bool IsRunning => _running && _started.HasValue;
    public override bool IsHighResolution => false;

    public override void Start()
    {
        if (_started == null)
        {
            _started = timeProvider.GetTimestamp();
            _running = true;
        }
    }

    public override void Stop()
    {
        if (_running)
        {
            _stopped = timeProvider.GetElapsedTime(_started ?? 0);
            _running = false;
        }
    }
}
