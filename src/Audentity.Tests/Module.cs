using System.Runtime.CompilerServices;

namespace Audentity.Tests;

public static class Module
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifierSettings.UseStrictJson();
        VerifierSettings.DontIgnoreEmptyCollections();
    }
}