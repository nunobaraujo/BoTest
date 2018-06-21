using MessagePack;
using System;

namespace Contracts.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class FrontTerminal : IFrontTerminal
    {
        public string Id {get;set;}

        public DateTime CreationDate {get;set;}

        public string Description {get;set;}

        public string LastAddress {get;set;}

        public DateTime? LastConnection {get;set;}

        public string MachineName {get;set;}

        public string TerminalId {get;set;}

        public string TerminalOptions {get;set;}
    }
}
