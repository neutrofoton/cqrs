using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CQRS.Core.Commands.Infrastructures
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly Dictionary<Type, Func<CommandMessage, Task>> _handlers = new();

        public void RegisterHandler<T>(Func<T, Task> handler) where T : CommandMessage
        {
            if (_handlers.ContainsKey(typeof(T)))
            {
                throw new IndexOutOfRangeException("You cannot register the same command handler twice!");
            }

            _handlers.Add(typeof(T), x => handler((T)x));
        }

        public async Task SendAsync(CommandMessage command)
        {
            if (_handlers.TryGetValue(command.GetType(), out Func<CommandMessage, Task> handler))
            {
                await handler(command);
            }
            else
            {
                throw new ArgumentNullException(nameof(handler), "No command handler was registered!");
            }
        }
    }
}