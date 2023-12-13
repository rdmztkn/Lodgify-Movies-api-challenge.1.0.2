using ApiApplication.Core.Domain.Entities;
using System;
using System.Collections.Generic;

namespace ApiApplication.Core.Application.Models.ViewModels
{
    public class TicketViewModel
    {
        public Guid Id { get; set; }
        public int NumberOfSeats { get; set; }
        public MovieViewModel Movie { get; set; }
        public AuditoriumViewModel Auditorium { get; set; }
    }
}