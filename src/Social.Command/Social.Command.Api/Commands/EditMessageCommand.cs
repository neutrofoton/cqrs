using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Commands;

namespace Social.Command.Api.Commands
{
    public class EditMessageCommand : CommandMessage
    {
        public string Message { get; set; }
    }
}