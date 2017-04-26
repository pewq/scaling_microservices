using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scaling_microservices.StorageStub
{
    public abstract class IStorage<T> : List<T>
    {
        abstract public T GetById(int id);
    }
}
