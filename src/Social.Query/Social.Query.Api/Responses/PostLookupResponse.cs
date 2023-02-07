using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Social.Query.Domain.Entities;
using Social.Shared.DTOs;

namespace Social.Query.Api.Responses
{
    public class PostLookupResponse : BaseResponse
    {
        public List<PostEntity> Posts { get; set; }
    }
}