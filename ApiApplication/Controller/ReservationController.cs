using ApiApplication.Core.Application.Features.Commands.ConfirmReservation;
using ApiApplication.Core.Application.Features.Commands.CreateReservation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ApiApplication.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IMediator mediator;

        public ReservationController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReservationCommand command)
        {
            var result = await mediator.Send(command);

            return Created("", result);
        }

        [HttpPost("Confirm/{id}")]
        public async Task<IActionResult> Confirm(Guid id)
        {
            var result = await mediator.Send(new ConfirmReservationCommand() { ReservationId = id });

            return Ok(result);
        }
    }
}