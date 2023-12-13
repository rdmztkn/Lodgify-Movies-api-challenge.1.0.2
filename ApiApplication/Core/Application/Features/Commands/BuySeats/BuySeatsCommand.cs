using MediatR;
using System;

namespace ApiApplication.Core.Application.Features.Commands.BuySeats
{
    public class BuySeatsCommand : IRequest<bool>
    {
        public Guid ReservationId { get; set; }
    }
}