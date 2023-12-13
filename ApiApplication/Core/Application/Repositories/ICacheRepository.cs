using System.Threading.Tasks;

namespace ApiApplication.Core.Application.Repositories
{
    public interface ICacheRepository
    {
        Task<T> Get<T>(string key);

        Task Set<T>(string key, T value);
    }
}