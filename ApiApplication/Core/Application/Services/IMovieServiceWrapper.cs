using ApiApplication.Core.Application.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiApplication.Core.Application.Services
{
    public interface IMovieServiceWrapper
    {
        Task<List<MovieWrapperViewModel>> GetAllMovies();
        Task<MovieWrapperViewModel> GetMovieById(string id);
    }
}