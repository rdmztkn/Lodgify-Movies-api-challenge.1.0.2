using ApiApplication.Core.Application.Repositories;
using ApiApplication.Exceptions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ApiApplication.Core.Application.Features.Commands.ConfirmReservation
{
    public class ConfirmReservationCommandHandler : IRequestHandler<ConfirmReservationCommand, bool>
    {
        private readonly ITicketsRepository ticketsRepository;
        //private readonly IMediator mediator;

        public ConfirmReservationCommandHandler(ITicketsRepository ticketsRepository/*, IMediator mediator*/)
        {
            this.ticketsRepository = ticketsRepository;
            //this.mediator = mediator;
        }

        public async Task<bool> Handle(ConfirmReservationCommand request, CancellationToken cancellationToken)
        {
            //var buyable = await mediator.Send(new BuySeatsCommand { ReservationId = request.ReservationId }, cancellationToken);
            //if (!buyable)
            //    throw new Exception("Reservation not buyable");

            if (request is null || request.ReservationId == Guid.Empty)
                throw new ReservationException("Reservation not found");

            var ticket = await ticketsRepository.GetAsync(request.ReservationId, cancellationToken);

            if (ticket is null)
                throw new ReservationException("Reservation not found");

            if (ticket.CreatedTime.IsExpired())
                throw new ReservationException("Reservation expired");

            if (ticket.Paid)
                throw new ReservationException("Reservation already paid");

            var entity = await ticketsRepository.ConfirmPaymentAsync(ticket, cancellationToken);

            if (entity is null || entity.Id == Guid.Empty)
                throw new NotCreatedException("Reservation couldn't confirmed");

            return entity is not null;
        }
    }
}