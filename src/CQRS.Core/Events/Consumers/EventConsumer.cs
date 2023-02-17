using CQRS.Core.Events.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRS.Core.Events.Consumers
{
    public abstract class EventConsumer : IEventConsumer
    {
        private readonly IEventListenerHandler _eventListenerHandler;
        public EventConsumer(IEventListenerHandler eventHandler)
        {
            _eventListenerHandler = eventHandler;
        }

        public IEventListenerHandler EventListenerHandler => _eventListenerHandler;

        protected object ExecuteEventHandler(EventMessage @event)
        {
            return EventListenerHandler.Process(@event);
        }
        public abstract void Consume(string topic);
    }
}
