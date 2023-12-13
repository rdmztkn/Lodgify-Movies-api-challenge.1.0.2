using System;

namespace ApiApplication.Core.Domain.Entities
{
    public class MovieViewModel
    {
        public int Id { get; set; }
        public string ServiceId { get; set; } // ProvidedApi Id

        public string Title { get; set; }
        public string ImdbId { get; set; }
        public string Stars { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}