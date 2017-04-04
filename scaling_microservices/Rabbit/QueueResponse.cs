using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace scaling_microservices.Rabbit
{
    [Serializable]
    public class QueueResponse
    {
        public static string classname { get { return typeof(QueueResponse).ToString(); } }

        public string message { get; set; }
        public object body { get; set; }

        public IBasicProperties properties { get; set; }

        public QueueResponse(byte[] data)
        {
            var formatter = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                ms.Write(data, 0, data.Length);
                ms.Seek(0, SeekOrigin.Begin);
                var temp = formatter.Deserialize(ms) as QueueResponse;
                this.message = temp.message;
                this.body = temp.body;
                this.properties = temp.properties;
            }
        }

        public byte[] ToByteArray()
        {
            var formatter = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                formatter.Serialize(ms, this);
                return ms.ToArray();
            }
        }

        public QueueResponse(byte[] data, IBasicProperties props) : this(data)
        {
            properties = props;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this.body);
        }
    }
}
