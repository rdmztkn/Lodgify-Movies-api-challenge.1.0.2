using ApiApplication.Core.Application.Models.RequestModels;
using ApiApplication.Core.Application.Models.ViewModels;
using ApiApplication.Core.Application.Repositories;
using ApiApplication.Core.Domain.Entities;
using ApiApplication.Exceptions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Server.IIS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ApiApplication.Core.Application.Features.Commands.CreateReservation
{
    public class CreateReservationCommandHandler : IRequestHandler<CreateReservationCommand, TicketViewModel>
    {
        /*
        - It should not be possible to reserve the same seats two times in 10 minutes.
        - It shouldn't be possible to reserve an already sold seat.
        - All the seats, when doing a reservation, need to be contiguous.
         */

        private readonly ITicketsRepository ticketsRepository;
        private readonly IMapper mapper;
        private readonly IShowtimesRepository showtimesRepository;

        public CreateReservationCommandHandler(ITicketsRepository ticketsRepository, IMapper mapper, IShowtimesRepository showtimesRepository)
        {
            this.ticketsRepository = ticketsRepository;
            this.mapper = mapper;
            this.showtimesRepository = showtimesRepository;
        }

        public async Task<TicketViewModel> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
        {
            static bool AreAnySeatsContigious(IEnumerable<SeatRequestModel> seats)
            {
                if (seats is null || !seats.Any())
                    throw new NotFoundException("Seats not found");

                var sortedSeats = seats.OrderBy(s => s.Row).ThenBy(s => s.SeatNumber).ToList();

                for (int i = 0; i < sortedSeats.Count - 1; i++)
                {
                    if (sortedSeats[i].Row == sortedSeats[i + 1].Row &&
                        Math.Abs(sortedSeats[i].SeatNumber - sortedSeats[i + 1].SeatNumber) == 1)
                    {
                        return true;
                    }
                }

                return false;
            }

            bool isContiguous = AreAnySeatsContigious(request.Seats);

            if (!isContiguous)
            {
                throw new ContigiousSeatsException("Seats are not contiguous");
            }

            var showTimes = await showtimesRepository.GetAllAsync(i => i.Id == request.ShowtimeId, cancellationToken);
            if (showTimes is null || !showTimes.Any())
                throw new NotFoundException($"Showtime with id {request.ShowtimeId} not found");

            var showTime = showTimes.FirstOrDefault();

            var tickets = await ticketsRepository.GetTicketsBySeats(request.ShowtimeId,
                                                                    request.Seats,
                                                                    cancellationToken);

            if (tickets.Any())
            {
                var paidOrNotExpired = tickets.Where(i => i.Paid || !i.CreatedTime.IsExpired());
                if (paidOrNotExpired is not null && paidOrNotExpired.Any())
                {
                    var seatNumbers = paidOrNotExpired.SelectMany(i => i.TicketSeats).Select(i => $"{i.Row}/{i.SeatNumber}");
                    var message = $"Seats ({string.Join(',', seatNumbers)}) are reserved already";
                    throw new ReservationException(message);
                }
            }

            var ticketEntity = new TicketEntity()
            {
                CreatedTime = DateTime.Now,
                Paid = false,
                ShowtimeId = request.ShowtimeId,
            };

            var seatEntities = request.Seats.Select(i => new SeatEntity()
            {
                AuditoriumId = showTime.AuditoriumId,
                Row = i.Row,
                SeatNumber = i.SeatNumber,
            });

            var ticket = await ticketsRepository.CreateAsync(showTime, seatEntities, cancellationToken);

            if (ticket is null || ticket.Id == Guid.Empty)
                throw new NotCreatedException("Ticket not created");

            var result = mapper.Map<TicketViewModel>(ticket);

            return result;
        }
    }
}