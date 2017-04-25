using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scaling_microservices.Registry
{
    class RegistryEntry : IComparable
    {
        public string Id { get; set; }

        public string Address { get; set; }

        public string Token { get; set; } //unused

        //TODO : maybe extract enum ServiceType
        public string ServiceType { get; set; }

        public string Owner { get; set; }

        public DateTime? Expiry { get; private set; } //TODO : is this really needed?

        static private bool EqualOrNone(string left, string right)
        {
            if (left == "" || right == "")
                return true;
            return left == right;
        }

        public RegistryEntry(int offset = ServiceRegistry.DefaultTimeout)
        {
            Expiry = DateTime.Now.AddSeconds(offset);
        }

        public RegistryEntry(DateTime expiryDate)
        {
            if(expiryDate < DateTime.Now)
            {
                throw new Exception("RegistryEntry(DateTime) : date of expiration should be bigger than current date");
            }
            Expiry = expiryDate;
        }

        public void Reset(int offset = ServiceRegistry.DefaultTimeout)
        {
            Expiry = DateTime.Now.AddSeconds(offset);
        }

        #region IComparable

        //for sorting by expiration time
        public int CompareTo(object obj)
        {
            try
            {
                return Expiry.Value.CompareTo((obj as RegistryEntry).Expiry.Value);
            }
            catch(InvalidCastException)
            {
                throw new ArgumentException("cannot cast to RegistryEntry");
            }
            catch(InvalidOperationException)
            {
                throw new ArgumentException("object has no value");
            }
        }

        #endregion IComparable
        #region Equals

        public static bool operator == (RegistryEntry first, RegistryEntry second)
        {
            return (first.Id == second.Id && first.Address == second.Address && first.Owner == second.Owner) &&           
                EqualOrNone(first.ServiceType, second.ServiceType) &&
                EqualOrNone(first.Token, second.Token);
        }

        public static bool operator != (RegistryEntry first, RegistryEntry second)
        {
            return !(first == second);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null) || ReferenceEquals(this, null))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            try
            {
                return this == (obj as RegistryEntry);
            }
            catch(InvalidCastException)
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return ServiceType.GetHashCode();
        }
        #endregion
    }
}