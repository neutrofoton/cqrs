using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Commands;

namespace Social.Command.Api.Commands
{
    public class NewPostCommand : CommandMessage
    {
        public string Author { get; set; }
        public string Message { get; set; }
    }
}