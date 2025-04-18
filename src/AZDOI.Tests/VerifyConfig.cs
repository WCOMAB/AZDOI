using System.Runtime.CompilerServices;
using VerifyTests.DiffPlex;

namespace AZDOI.Tests;

public static class VerifyConfig
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifyDiffPlex.Initialize(OutputType.Compact);
        VerifierSettings.AddExtraSettings(settings =>
        {
            settings.Converters.Add(new FakeLogRecordConverter());
        });
    }
}