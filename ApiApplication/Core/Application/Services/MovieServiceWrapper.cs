using ApiApplication.Core.Application.Models.ViewModels;
using ApiApplication.Core.Application.Repositories;
using AutoMapper;
using Google.Protobuf.Collections;
using Microsoft.Extensions.DependencyInjection;
using ProtoDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiApplication.Core.Application.Services
{
    public class MovieServiceWrapper : IMovieServiceWrapper
    {
        private const string MOVIES_KEY = "all_movies";
        private readonly ApiClientGrpc grpcClient;
        private readonly ICacheRepository cacheRepository;
        private readonly IMapper mapper;

        public MovieServiceWrapper(ICacheRepository cacheRepository, ApiClientGrpc grpcClient, IMapper mapper)
        {
            this.cacheRepository = cacheRepository;
            this.grpcClient = grpcClient;
            this.mapper = mapper;
        }

        public async Task<List<MovieWrapperViewModel>> GetAllMovies()
        {
            showListResponse response;
            List<MovieWrapperViewModel> result = null;

            try
            {
                response = await grpcClient.GetAll();
                result = ConvertToViewModelList(response.Shows);
                await cacheRepository.Set(MOVIES_KEY, result);
            }
            catch
            {
                return await GetMoviesFromCache();
            }

            return result;
        }

        public async Task<MovieWrapperViewModel> GetMovieById(string id)
        {
            showResponse response = null;
            try
            {
                response = await grpcClient.GetById(id);

                if (response is null)
                    throw new Exception("Movie not found");
            }
            catch
            {
                var movies = await GetMoviesFromCache();
                return movies.FirstOrDefault(m => m.Id == id);
            }

            var result = mapper.Map<MovieWrapperViewModel>(response);
            return result;
        }

        private Task<List<MovieWrapperViewModel>> GetMoviesFromCache()
        {
            return cacheRepository.Get<List<MovieWrapperViewModel>>(MOVIES_KEY);
        }

        private List<MovieWrapperViewModel> ConvertToViewModelList(RepeatedField<showResponse> response)
        {
            return mapper.Map<List<MovieWrapperViewModel>>(response);
        }
    }
}