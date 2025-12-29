using System;
using Lexy.Compiler.Language.Functions;

namespace Lexy.Compiler.FunctionLibraries;

public interface ILibrary
{
    Type Type { get; }
    IInstanceFunction GetFunction(string identifier);
}