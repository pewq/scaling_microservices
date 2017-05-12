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
    public class QueueRequest
    {
        public static string classname { get { return typeof(QueueRequest).ToString(); } }

        public string method { get; set; }

        public void SetMethod(string method)
        {
            this.method = method;
        }

        public IBasicProperties properties { get; set; }
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

        public QueueRequest(byte[] bytes, IBasicProperties props) : this(bytes)
        {
            properties = props;
        }

        public string this[string key]
        {
            get
            {
                return arguments[key];
            }
            set
            {
                arguments[key] = value;
            }
        }

        public string Contains(string key)
        {
            if(this.arguments.ContainsKey(key))
            {
                return this[key];
            }
            return null;
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