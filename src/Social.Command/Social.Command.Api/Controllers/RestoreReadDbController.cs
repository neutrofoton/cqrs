using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Commands.Infrastructures;
using CQRS.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Social.Command.Api.Commands;
using Social.Command.Api.Responses;
using Social.Shared.DTOs;

namespace Social.Command.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RestoreReadDbController : ControllerBase
    {
        private readonly ILogger<RestoreReadDbController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;

        public RestoreReadDbController(ILogger<RestoreReadDbController> logger, ICommandDispatcher commandDispatcher)
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
        }

        [HttpPost]
        public async Task<ActionResult> RestoreReadDbAsync()
        {
            try
            {
                await _commandDispatcher.SendAsync(new RestoreReadDbCommand());

                return StatusCode(StatusCodes.Status201Created, new BaseResponse
                {
                    Message = "Read database restore request completed successfully!"
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.Log(LogLevel.Warning, ex, "Client made a bad request!");
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to restore read database!";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }
    }
}