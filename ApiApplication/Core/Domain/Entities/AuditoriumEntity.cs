using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiApplication.Core.Domain.Entities
{
    public class AuditoriumEntity
    {
        public int Id { get; set; }

        public ICollection<ShowtimeEntity> Showtimes { get; set; }

        public ICollection<SeatEntity> Seats { get; set; }
    }
}