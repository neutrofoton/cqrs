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
        private readonly IEventTargetHandler _eventHandler;
        public EventConsumer(IEventTargetHandler eventHandler)
        {
            _eventHandler = eventHandler;
        }

        public IEventTargetHandler EventHandler => _eventHandler;

        public abstract void Consume(string topic);
    }
}
