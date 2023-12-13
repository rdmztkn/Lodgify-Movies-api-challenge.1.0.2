using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using ApiApplication.Core.Domain.Entities;
using ApiApplication.Core.Application.Repositories;
using ApiApplication.Infrastructure.Infrastructure.Persistence;
using ApiApplication.Core.Application.Models.RequestModels;
using ApiApplication.Exceptions;
using System.Net.Sockets;

namespace ApiApplication.Infrastructure.Infrastructure.Persistence.Repositories
{
    public class TicketsRepository : ITicketsRepository
    {
        private readonly CinemaContext context;

        public TicketsRepository(CinemaContext context)
        {
            this.context = context;
        }

        public Task<TicketEntity> GetAsync(Guid id, CancellationToken cancel)
        {
            return context.Tickets.FirstOrDefaultAsync(x => x.Id == id, cancel);
        }

        public async Task<IEnumerable<TicketEntity>> GetEnrichedAsync(int showtimeId, CancellationToken cancel)
        {
            return await context.Tickets
                .Include(x => x.Showtime)
                .Include(x => x.TicketSeats)
                .Where(x => x.ShowtimeId == showtimeId)
                .ToListAsync(cancel);
        }


        public async Task<IEnumerable<TicketEntity>> GetTicketsBySeats(int showtimeId, List<SeatRequestModel> seats, CancellationToken cancel)
        {
            var rows = seats.Select(x => x.Row);
            var seatNumbers = seats.Select(x => x.SeatNumber);

            return await context.Tickets
                            .Include(i => i.TicketSeats)
                            .Include(x => x.Showtime)
                            .Where(x => x.ShowtimeId == showtimeId)
                            .Where(x => x.TicketSeats.Any(s => rows.Contains(s.Row)))
                            .Where(x => x.TicketSeats.Any(s => seatNumbers.Contains(s.SeatNumber)))
                            .ToListAsync(cancel);
        }

        public async Task<TicketEntity> CreateAsync(ShowtimeEntity showtime, IEnumerable<SeatEntity> selectedSeats, CancellationToken cancel)
        {
            var seats = new List<SeatEntity>(selectedSeats);

            var ticketEntity = new TicketEntity
            {
                ShowtimeId = showtime.Id
            };

            var ticketSeats = seats.Select(x => new TicketSeatsEntity
            {
                Ticket = ticketEntity,
                AuditoriumId = showtime.AuditoriumId,
                Row = x.Row,
                SeatNumber = x.SeatNumber
            });

            context.TicketSeats.AddRange(ticketSeats);

            var affectedRows = await context.SaveChangesAsync(cancel);

            return affectedRows == 0 ? throw new ReservationException("Error while creating ticket") : ticketEntity;
        }

        public async Task<TicketEntity> ConfirmPaymentAsync(TicketEntity ticket, CancellationToken cancel)
        {
            ticket.Paid = true;
            context.Update(ticket);
            var affectedRows = await context.SaveChangesAsync(cancel);

            return affectedRows == 0 ? throw new ReservationException("Error while confirming payment") : ticket;
        }
    }
}