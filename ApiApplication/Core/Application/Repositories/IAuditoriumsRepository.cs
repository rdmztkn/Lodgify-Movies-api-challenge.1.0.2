using ApiApplication.Core.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ApiApplication.Core.Application.Repositories
{
    public interface IAuditoriumsRepository
    {
        Task<AuditoriumEntity> GetAsync(int auditoriumId, CancellationToken cancel);
    }
}