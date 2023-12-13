using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using System.Linq.Expressions;
using ApiApplication.Core.Domain.Entities;
using ApiApplication.Core.Application.Repositories;
using ApiApplication.Infrastructure.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Internal;

namespace ApiApplication.Infrastructure.Infrastructure.Persistence.Repositories
{
    public class ShowtimesRepository : IShowtimesRepository
    {
        private readonly CinemaContext context;

        public ShowtimesRepository(CinemaContext context)
        {
            this.context = context;
        }

        public async Task<ShowtimeEntity> GetWithMoviesByIdAsync(int id, CancellationToken cancel)
        {
            return await context.Showtimes
                .Include(x => x.Movie)
                .FirstOrDefaultAsync(x => x.Id == id, cancel);
        }

        public async Task<ShowtimeEntity> GetWithTicketsByIdAsync(int id, CancellationToken cancel)
        {
            return await context.Showtimes
                .Include(x => x.Tickets)
                .FirstOrDefaultAsync(x => x.Id == id, cancel);
        }

        public async Task<IEnumerable<ShowtimeEntity>> GetAllAsync(Expression<Func<ShowtimeEntity, bool>> filter = null, CancellationToken cancel = default)
        {
            var query = context.Showtimes.AsQueryable();

            if (filter != null)
                query = query.Where(filter);

            return await query
                            .Include(x => x.Movie)
                            .Include(x => x.Auditorium)
                            .ToListAsync(cancel);
        }

        public async Task<ShowtimeEntity> CreateShowtime(ShowtimeEntity showtimeEntity, CancellationToken cancel)
        {
            var showtime = await context.Showtimes.AddAsync(showtimeEntity, cancel);
            await context.SaveChangesAsync(cancel);
            return showtime.Entity;
        }
    }
}