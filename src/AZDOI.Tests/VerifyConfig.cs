using System.Runtime.CompilerServices;
using VerifyTests.DiffPlex;

namespace AZDOI.Tests;

public static class VerifyConfig
{
    [ModuleInitializer]
    public static void Init()
    {
       VerifierSettings.InitializePlugins();
       VerifyDiffPlex.Initialize(OutputType.Compact);
    }
}