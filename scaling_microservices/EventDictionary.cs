using System;
using System.Collections.Generic;

namespace scaling_microservices
{
    public delegate void Handler<T>(T args);

    public class EventDictionary<T> : Dictionary<string, System.Delegate>
    {
        //TODO : add lock
        object Lock = new object();
        
        public void Handle(string name, EventArgs args)
        {
            base[name].DynamicInvoke(args);
        }

        public void Add(string name, Handler<T> function)
        {
            lock (Lock)
            {
                if (!base.ContainsKey(name))
                {
                    base[name] = null;
                }
                base[name] = (Handler<T>)base[name] + function;
            }
        }

        public EventDictionary() : base() { }
    }
}
