using System;
using System.Linq;
using System.Collections.Generic;

namespace scaling_microservices
{
    class QueueRequest
    {
        public string method { get; private set; }

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
    }
}