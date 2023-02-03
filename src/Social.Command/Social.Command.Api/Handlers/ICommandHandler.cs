using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Social.Command.Api.Commands;

namespace Social.Command.Api.Handlers
{
    public interface ICommandHandler
    {
        Task HandleAsync(NewPostCommand command);
        Task HandleAsync(EditMessageCommand command);
        Task HandleAsync(LikePostCommand command);
        Task HandleAsync(AddCommentCommand command);
        Task HandleAsync(EditCommentCommand command);
        Task HandleAsync(RemoveCommentCommand command);
        Task HandleAsync(DeletePostCommand command);
        Task HandleAsync(RestoreReadDbCommand command);
    }
}