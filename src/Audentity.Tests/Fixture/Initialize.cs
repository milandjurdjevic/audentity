using System.Runtime.CompilerServices;

namespace Audentity.Tests.Fixture;

public static class Initialize
{
    [ModuleInitializer]
    public static void Module()
    {
        VerifierSettings.UseStrictJson();
        VerifierSettings.DontIgnoreEmptyCollections();
    }
}