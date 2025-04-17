using System.Runtime.CompilerServices;

namespace AZDOI.Tests;

public static class VerifyConfig
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifierSettings.AddExtraSettings(settings =>
        {
            settings.Converters.Add(new FakeLogRecordConverter());
        });
    }
}