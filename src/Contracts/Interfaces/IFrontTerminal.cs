using System;

namespace Contracts
{
    public interface IFrontTerminal
    {
        string Id { get; }

        DateTime CreationDate { get; }
        string Description { get; }
        
        string LastAddress { get; }
        DateTime? LastConnection { get; }
        string MachineName { get; }
        string TerminalId { get; }
        string TerminalOptions { get; }
    }
}