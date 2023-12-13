using System.Collections.Generic;

namespace ApiApplication.Core.Domain.Entities
{
    public class SeatEntity
    {
        public short Row { get; set; }
        public short SeatNumber { get; set; }
        public int AuditoriumId { get; set; }
        public AuditoriumEntity Auditorium { get; set; }

        public ICollection<TicketSeatsEntity> TicketSeats { get; set; }
    }
}