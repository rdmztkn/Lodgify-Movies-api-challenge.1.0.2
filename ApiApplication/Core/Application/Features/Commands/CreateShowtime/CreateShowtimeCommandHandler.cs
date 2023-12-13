using ApiApplication.Core.Domain.Entities;
using System.Collections.Generic;
using MediatR;
using ApiApplication.Core.Application.Repositories;
using System.Threading.Tasks;
using System.Threading;
using ApiApplication.Core.Application.Services;
using AutoMapper;
using ApiApplication.Exceptions;
using System.Linq;
using System;

namespace ApiApplication.Core.Application.Features.Commands.CreateShowtime
{
    public class CreateShowtimeCommandHandler : IRequestHandler<CreateShowtimeCommand, int>
    {
        private readonly IShowtimesRepository showtimesRepository;
        private readonly IMovieServiceWrapper movieServiceWrapper;
        private readonly IMapper mapper;

        public CreateShowtimeCommandHandler(IShowtimesRepository showtimesRepository,
                                            IMovieServiceWrapper movieServiceWrapper,
                                            IMapper mapper)
        {
            this.showtimesRepository = showtimesRepository;
            this.movieServiceWrapper = movieServiceWrapper;
            this.mapper = mapper;
        }

        public async Task<int> Handle(CreateShowtimeCommand request, CancellationToken cancellationToken)
        {
            var movie = await movieServiceWrapper.GetMovieById(request.MovieId)
                        ?? throw new NotFoundException($"Movie with id {request.MovieId} not found");

            var showtime = new ShowtimeEntity
            {
                Movie = mapper.Map<MovieEntity>(movie),
                SessionDate = request.SessionDate,
                AuditoriumId = request.AuditoriumId,
                Tickets = Array.Empty<TicketEntity>()
            };

            var result = await showtimesRepository.CreateShowtime(showtime, cancellationToken);

            if (result.Id == 0)
                throw new NotCreatedException("Showtime not created");

            return result.Id;
        }
    }
}