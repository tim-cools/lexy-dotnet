using Lexy.Compiler.Parser;

namespace Lexy.Tests;

public record ParseResult<T>(T Result, IParserLogger Logger);