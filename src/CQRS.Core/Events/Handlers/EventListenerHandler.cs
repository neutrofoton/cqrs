using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRS.Core.Events.Handlers
{
    public abstract class EventListenerHandler : IEventListenerHandler
    {
        public object Process(EventMessage @event)
        {
            var handlerMethod = this.GetType().GetMethod("On", new Type[] { @event.GetType() });

            if (handlerMethod == null)
            {
                throw new ArgumentNullException(nameof(handlerMethod), "Could not find event handler method!");
            }

            return handlerMethod.Invoke(this, new object[] { @event });
        }
    }
}
