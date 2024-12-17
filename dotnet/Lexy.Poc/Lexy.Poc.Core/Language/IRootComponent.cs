using System;

namespace Lexy.Poc.Core.Language
{
    public interface IRootComponent : IComponent
    {
        string TokenName { get; }
        void Fail(string exception);
    }
}