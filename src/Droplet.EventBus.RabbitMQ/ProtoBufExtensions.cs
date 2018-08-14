using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Droplet.EventBus.RabbitMQ
{
    public static class ProtoBufExtensions
    {
        public static T DeserializeProtoBuf<T>(this byte[] data)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                return Serializer.Deserialize<T>((Stream)memoryStream);
            }
        }

        public static object DeserializeProtoBuf(this byte[] data,Type type)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                return Serializer.Deserialize(type,(Stream)memoryStream);
            }
        }

        public static byte[] ToProtoBufBytes<T>(this T input) where T : class
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Serializer.Serialize<T>((Stream)memoryStream, input);
                return memoryStream.ToArray();
            }
        }
    }
}
