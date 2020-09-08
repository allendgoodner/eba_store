using System;
using System.Collections.Generic;
using System.Text;

namespace EbaLibrary
{
    public interface IEventStore<T>
    {
        public IEnumerable<T> GetEvents(Guid aggregateId);
        void PersistEvents(IEnumerable<T> newEvents);
    }
}
