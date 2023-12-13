using ApiApplication.Core.Application.Features.Commands.BuySeats;
using MediatR;
using System;

namespace ApiApplication.Core.Application.Features.Commands.ConfirmReservation
{
    public class ConfirmReservationCommand : IRequest<bool>
    {
        public Guid ReservationId { get; set; }
    }
}