using ApiApplication.Core.Application.Features.Commands.CreateShowtime;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ApiApplication.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowtimeController : ControllerBase
    {
        private readonly IMediator mediator;

        public ShowtimeController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateShowtimeCommand command)
        {
            var response = await mediator.Send(command);

            return Ok(response);
        }
    }
}