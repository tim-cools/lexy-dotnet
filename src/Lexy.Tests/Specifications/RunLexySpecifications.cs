using Lexy.Compiler.Specifications;
using NUnit.Framework;

namespace Lexy.Tests.Specifications;

public class RunLexySpecifications : ScopedServicesTestFixture
{
    [Test]
    public void AllSpecifications()
    {
        LoggingConfiguration.LogFileNames();

        var runner = GetService<ISpecificationsRunner>();
        runner.RunAll("../../../lexy-language/src/Specifications");
    }

    [Test]
    public void SpecificFile() // used for debugging a specific file from IDE
    {
        LoggingConfiguration.LogFileNames();

        var runner = GetService<ISpecificationsRunner>();
        runner.Run("../../../lexy-language/src/Specifications/Function/ComplexVariables.lexy");

        //runner.Run("/Users/timcools/_/Lexy/lexy-language/src/Specifications/Table/Syntax.lexy");
        //runner.Run("../../../lexy-language/src/Specifications/Isolate.lexy");
        //runner.Run("../../../lexy-language/src/Specifications/Function/Variables.lexy");
        //runner.Run("../../../lexy-language/src/Specifications/BuiltInFunctions/Extract.lexy");
    }
}