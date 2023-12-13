using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiApplication.Core.Domain.Entities
{
    public class MovieEntity
    {
        public int Id { get; set; }

        public string ServiceId { get; set; } // ProvidedApi Id

        public string Title { get; set; }
        public string ImdbId { get; set; }
        public string Stars { get; set; }
        public DateTime ReleaseDate { get; set; }

        public ICollection<ShowtimeEntity> Showtimes { get; set; }
    }
}