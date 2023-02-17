using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Events;

namespace CQRS.Core.Domains
{
    public abstract class AggregateRoot<TId>
    {
        protected TId _id;
        private bool _active;
        
        private readonly List<EventMessage> _changes = new();

        public TId Id
        {
            get { return _id; }
        }

        public int Version { get; set; } = -1;
        
        public bool Active { get => _active; set => _active = value; }

        public IEnumerable<EventMessage> GetUncommittedChanges()
        {
            return _changes;
        }

        public void MarkChangesAsCommitted()
        {
            _changes.Clear();
        }

        private void ApplyChange(EventMessage @event, bool isNew)
        {
            var method = this.GetType().GetMethod("Apply", new Type[] { @event.GetType() });

            if (method == null)
            {
                throw new ArgumentNullException(nameof(method), $"The Apply method was not found in the aggregate for {@event.GetType().Name}!");
            }

            method.Invoke(this, new object[] { @event });

            if (isNew)
            {
                _changes.Add(@event);
            }
        }

        protected void RaiseEvent(EventMessage @event)
        {
            ApplyChange(@event, true);
        }

        public void ReplayEvents(IEnumerable<EventMessage> events)
        {
            foreach (var @event in events)
            {
                ApplyChange(@event, false);
            }
        }
    }
}