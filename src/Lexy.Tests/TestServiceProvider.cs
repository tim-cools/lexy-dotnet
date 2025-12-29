using NUnit.Framework;

namespace Lexy.Tests;

[SetUpFixture]
public class TestServiceProvider
{
    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        LoggingConfiguration.RemoveOldFiles();
        LoggingConfiguration.ConfigureSerilog();
    }
}