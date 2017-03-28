using System;
using System.Collections.Generic;

namespace scaling_microservices
{
    public delegate void Handler<T>(T args);

    public class EventDictionary<T> : Dictionary<string, System.Delegate>
    {
        object Lock = new object();
        
        public new System.Delegate this[string key]
        {
            get
            {
                lock(Lock)
                {
                    return base[key];
                }
            }
            //TODO : is setter needed?
            //or is it better to replace with (AddEvent(name) {this[name] = null;}) ?
            set
            {
                lock(Lock)
                {
                    base[key] = value;
                }
            }
        }

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
