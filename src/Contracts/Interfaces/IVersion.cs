using System;

namespace Contracts
{
    public interface IVersion
    {
        string Version { get; }
        DateTime ReleaseDate { get; }
    }
}
