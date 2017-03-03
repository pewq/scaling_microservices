using System;
using System.Collections.Generic;
using System.Timers;
using System.Linq;

namespace scaling_microservices.Registry
{
    /// <summary>
    /// class, which contains active services and recieves
    /// pings from them in order to keep the connection
    /// </summary>
    class ServiceRegistry
    {
        private List<RegistryEntry> items;

        private List<RegistryEntry> candidates;

        private Timer registerTimer;

        private void RecalculateTimer()
        {
            if(items.Count == 0)
            {
                registerTimer.Enabled = false;
                return;
            }
            DateTime expiryTime = items.Min().Expiry.Value;
            candidates = items.Where(x => x.Expiry.Value == expiryTime).ToList();
            registerTimer.Enabled = true;
            registerTimer.Interval = expiryTime.Subtract(DateTime.Now).TotalMilliseconds;
        }
        
        public int timeout { get; }

        public ServiceRegistry(int timeoutInSeconds = ServiceRegistry.DefaultTimeout)
        {
            timeout = timeoutInSeconds;
            items = new List<RegistryEntry>();
            candidates = new List<RegistryEntry>();
            registerTimer = new Timer() { AutoReset = true, Enabled = false };
            registerTimer.Elapsed += (object sender, ElapsedEventArgs e) =>
            {
                foreach (var can in candidates)
                {
                    items.Remove(can);
                }
                this.RecalculateTimer();
            };
        }

        public const int DefaultTimeout = 60;

        public void Ping(string id, string token)
        {
            token = "";//todo : implement service tokens
            var item = items.Find( x => 
                ( x == new RegistryEntry() { Id = id, Token = token })
            );
            item.Reset();
        }

        public void Add(string id, string address, string token, string type)
        {
            RegistryEntry entry = new RegistryEntry()
                { Id = id, Address = address, Token = token, ServiceType = type };
            //todo : maybe change later to this.Contains
            RegistryEntry item = items.Find(x => x == entry);
            if(item != null)
            {
                item.Reset();
            }
            else
            {
                items.Add(entry);
                entry.Reset();
            }
            this.RecalculateTimer();
        }

        //add ContainsId, ContainsAddress etc. maybe general Contains with return type enum
        private bool Contains(RegistryEntry entry)
        {
            return false;
        }

        public Dictionary<string,DateTime> Get()
        {
            return items.Where(x => x.Expiry.HasValue)
                .ToDictionary(x => x.Id, x => x.Expiry.Value);
        }

        public List<string> GetServices()
        {
            return items.Select(x => x.Id).ToList();
        }


    }
}

//todo : add 3rdp-like registration behaviour

//example : 
/*
 * int timeout = 100;
 * var registry = new ServiceRegistry(timeout);
 * registry.Add("service-token");
 * registry.Get().Count; // == 1
 * Thread.Wait((timeout + 10) * 1000); //110 seconds;
 * registry.Get().Count; // == 0
 */