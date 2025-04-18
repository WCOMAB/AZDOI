﻿using System.Runtime.CompilerServices;
using VerifyTests.DiffPlex;

namespace AZDOI.Tests;

public static class VerifyConfig
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifierSettings.DontIgnoreEmptyCollections();
        VerifierSettings.IgnoreStackTrace();
        VerifyDiffPlex.Initialize(OutputType.Compact);
        VerifierSettings.AddExtraSettings(settings =>
        {
            settings.DefaultValueHandling = Argon.DefaultValueHandling.Include;
            settings.Converters.Add(new FakeLogRecordConverter());
        });
    }
}