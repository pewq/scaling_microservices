using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

namespace scaling_microservices.Rabbit
{
    [Serializable]
    public class QueueRequest
    {
        public string method { get; set; }

        public void SetMethod(string method)
        {
            this.method = method;
        }
        public Dictionary<string, string> arguments { get; private set; }

        public QueueRequest(string method, string paramstring)
        {
            this.method = method;
            arguments = new Dictionary<string, string>();
            paramstring = paramstring.ToLower();
            arguments = paramstring.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split('='))
                .ToDictionary(x => x[0], x => x[1]);
        }

        public QueueRequest()
        {
            arguments = new Dictionary<string, string>();
        }
        public QueueRequest(byte[] bytes)
        {
            var formatter = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                ms.Write(bytes, 0, bytes.Length);
                ms.Seek(0, SeekOrigin.Begin);
                var obj = formatter.Deserialize(ms) as QueueRequest;
                this.method = obj.method;
                this.arguments = obj.arguments.ToDictionary(x=> x.Key, x=> x.Value);
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

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}