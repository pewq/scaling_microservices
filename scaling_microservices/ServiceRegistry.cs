using System;
using System.Collections.Generic;
using System.Timers;
using System.Linq;

namespace scaling_microservices
{
    /// <summary>
    /// class, which contains active services and recieves
    /// pings from them in order to keep the connection
    /// </summary>
    class ServiceRegistry
    {
        private Dictionary<string, DateTime> registry;

        private List<string> candidates;

        private Timer registerTimer;

        private void RecalculateTimer()
        {
            if(registry.Count == 0)
            {
                registerTimer.Enabled = false;
                return;
            }
            DateTime elapseTime = registry.Values.Min();
            candidates = registry.Keys.Where(x => registry[x] == elapseTime).ToList();
            registerTimer.Enabled = true;
            registerTimer.Interval = elapseTime.Subtract(DateTime.Now).TotalMilliseconds;
        }
        
        public int timeout { get; }

        public ServiceRegistry(int timeoutInSeconds = ServiceRegistry.DefaultTimeout) : base()
        {
            timeout = timeoutInSeconds;
            registry = new Dictionary<string, DateTime>();
            candidates = new List<string>();
            registerTimer = new Timer() { AutoReset = true, Enabled = false };
            registerTimer.Elapsed += (object sender, ElapsedEventArgs e) =>
            {
                foreach (string can in candidates)
                {
                    registry.Remove(can);
                }
                this.RecalculateTimer();
            };
        }

        public const int DefaultTimeout = 60;

        public void Add(string candidate)
        {
            if(registry.ContainsKey(candidate))
            {
                registry[candidate] = DateTime.Now.AddSeconds(timeout);
            }
            else
            {
                registry.Add(candidate, DateTime.Now.AddSeconds(timeout));
            }
            this.RecalculateTimer();
        }

        public Dictionary<string,DateTime> Get()
        {
            return registry.ToDictionary(e => e.Key, e => e.Value);
        }

        public List<string> GetServices()
        {
            return registry.Keys.ToList();
        }


    }
}

//example : 
/*
 * int timeout = 100;
 * var registry = new ServiceRegistry(timeout);
 * registry.Add("service-token");
 * registry.Get().Count; // == 1
 * Thread.Wait((timeout + 10) * 1000); //110 seconds;
 * registry.Get().Count; // == 0
 */