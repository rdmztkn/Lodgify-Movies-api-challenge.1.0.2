using ApiApplication.Core.Application.Repositories;
using ApiApplication.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace ApiApplication.Infrastructure.Infrastructure.Persistence.Repositories
{
    public class AuditoriumsRepository : IAuditoriumsRepository
    {
        private readonly CinemaContext context;

        public AuditoriumsRepository(CinemaContext context)
        {
            this.context = context;
        }

        public async Task<AuditoriumEntity> GetAsync(int auditoriumId, CancellationToken cancel)
        {
            return await context.Auditoriums
                            .Include(x => x.Seats)
                            .FirstOrDefaultAsync(x => x.Id == auditoriumId, cancel);
        }
    }
}