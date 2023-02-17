using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Messages;

namespace CQRS.Core.Events
{
    public abstract class EventMessage : Message
    {
        protected EventMessage(string type)
        {
            Type = type;
        }

        public string Type { get; set; }
        public int Version { get; set; }
    }
}