using System;
using System.Collections.Generic;

namespace scaling_microservices
{
    class __Request
    {
        public string method { get; private set; }
        public Dictionary<string, string> args { get; private set; }

        public Request(string request)
        {
            args = new Dictionary<string, string>();
            request = request.ToLower();
            string[] argStrings = request.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            if (argStrings.Length > 0)
            {
                string[] function = argStrings[0].Split('=');
                if (function[0] == "method")
                {
                    method = function[1];
                }
                for (int i = 1; i < argStrings.Length; ++i)
                {
                    string argStr = argStrings[i];
                    string[] argPair = argStr.Split('=');
                    args[argPair[0]] = argPair[1];
                }
            }
        }
    }
}