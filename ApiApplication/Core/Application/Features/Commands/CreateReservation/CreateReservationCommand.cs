using ApiApplication.Core.Domain.Entities;
using MediatR;
using System.Collections.Generic;
using System;
using ApiApplication.Core.Application.Models.ViewModels;
using ApiApplication.Core.Application.Models.RequestModels;

namespace ApiApplication.Core.Application.Features.Commands.CreateReservation
{
    public class CreateReservationCommand : IRequest<TicketViewModel>
    {
        public int ShowtimeId { get; set; }
        public List<SeatRequestModel> Seats { get; set; }
    }
}