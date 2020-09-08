using PubSub;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EbaLibrary
{
    public class EventHub
    {
        private Dictionary<Guid, IList<object>> _eventStreams;
        private Hub _hub;


        public EventHub(Hub hub)
        {
            _hub = hub;
            _eventStreams = new Dictionary<Guid, IList<object>>();
        }

        public void Subscribe(Action<object> action)
        {
            _hub.Subscribe<object>(action);
        }

        public IEnumerable<object> GetOrInitialize(Guid aggregateId)
        {
            if (!_eventStreams.ContainsKey(aggregateId)) _eventStreams.Add(aggregateId, new List<object>());
            return _eventStreams[aggregateId];
        }

        public void AddEvent(Guid aggregateId, object @event)
        {
            _eventStreams[aggregateId].Add(@event);
            _hub.Publish(@event);
        }


    }
}
