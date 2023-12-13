using System;
using MediatR;

namespace ApiApplication.Core.Application.Features.Commands.CreateShowtime
{
    public class CreateShowtimeCommand : IRequest<int>
    {
        public string MovieId { get; set; }
        public DateTime SessionDate { get; set; }
        public int AuditoriumId { get; set; }
    }
}