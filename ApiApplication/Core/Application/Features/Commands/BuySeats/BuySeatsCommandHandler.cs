using ApiApplication.Core.Application.Repositories;
using ApiApplication.Exceptions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ApiApplication.Core.Application.Features.Commands.BuySeats
{
    public class BuySeatsCommandHandler : IRequestHandler<BuySeatsCommand, bool>
    {
        private readonly ITicketsRepository ticketsRepository;

        public BuySeatsCommandHandler(ITicketsRepository ticketsRepository)
        {
            this.ticketsRepository = ticketsRepository;
        }

        public async Task<bool> Handle(BuySeatsCommand request, CancellationToken cancellationToken)
        {
            if (request is null || request.ReservationId == Guid.Empty)
                throw new ReservationException("Invalid reservation id");

            var ticket = await ticketsRepository.GetAsync(request.ReservationId, cancellationToken);

            if (ticket is null)
                throw new ReservationException("Reservation not found");

            if (ticket.CreatedTime.IsExpired())
                throw new ReservationException("Reservation expired");

            return true;
        }
    }
}