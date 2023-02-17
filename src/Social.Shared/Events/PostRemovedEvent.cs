using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Events;

namespace Social.Shared.Events
{
    public class PostRemovedEvent : EventMessage
    {
        public PostRemovedEvent() : base(nameof(PostRemovedEvent))
        {
        }
    }
}