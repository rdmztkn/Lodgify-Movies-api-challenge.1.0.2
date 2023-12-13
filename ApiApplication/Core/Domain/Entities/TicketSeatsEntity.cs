using System;
using System.Collections.Generic;

namespace ApiApplication.Core.Domain.Entities
{
    public class TicketSeatsEntity
    {
        public Guid Id { get; set; }

        public Guid TicketId { get; set; }

        public int AuditoriumId { get; set; }

        public short Row { get; set; }

        public short SeatNumber { get; set; }

        public TicketEntity Ticket { get; set; }
    }
}