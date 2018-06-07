using System;

namespace Services.Comms
{
    internal interface IClientConnection
    {
        DateTime ConnectionStart { get; }
        bool IsValid { get; }
        string SessionToken { get; }
        Guid Tag { get; }
    }
}