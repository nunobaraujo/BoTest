using NBsoft.Sockets;
using System;
using System.Collections.Generic;

namespace Services.Comms.Sockets
{
    internal class ClientConnection : IClientConnection
    {
        public string SessionToken { get; set; }
        public SocketClient Client { get; set; }
        public DateTime ConnectionStart { get; set; }
        public List<byte> IncomingMessage { get; set; }
        public Guid Tag { get; set; }
        public bool IsValid{ get; set; }
    }

}
