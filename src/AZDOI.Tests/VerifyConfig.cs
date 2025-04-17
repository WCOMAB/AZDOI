using System.Runtime.CompilerServices;

namespace AZDOI.Tests;

public static class VerifyConfig
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifierSettings.DontIgnoreEmptyCollections();
        VerifierSettings.IgnoreStackTrace();
        VerifierSettings.AddExtraSettings(settings =>
        {
            settings.DefaultValueHandling = Argon.DefaultValueHandling.Include;
            settings.Converters.Add(new FakeLogRecordConverter());
        });
    }
}
