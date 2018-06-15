using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    static class MessagePackSerializer
    {
        public static T Deserialize<T>(byte[] modelBytes)
        {
            return MessagePack.MessagePackSerializer.Deserialize<T>(modelBytes, MessagePack.Resolvers.ContractlessStandardResolver.Instance);
        }

        public static byte[] Serialize<T>(T model)
        {
            return MessagePack.MessagePackSerializer.Serialize(model, MessagePack.Resolvers.ContractlessStandardResolver.Instance);
        }
    }
}
