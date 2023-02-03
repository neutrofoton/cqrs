using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Social.Shared.DTOs;

namespace Social.Command.Api.Responses
{
    public class NewPostResponse : BaseResponse
    {
        public Guid Id { get; set; }
    }
}