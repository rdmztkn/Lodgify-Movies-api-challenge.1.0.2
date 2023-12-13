using ApiApplication.Core.Application.Repositories;
using ApiApplication.Core.Application.Services;
using ApiApplication.Infrastructure.Infrastructure.Persistence;
using ApiApplication.Infrastructure.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace ApiApplication.Infrastructure.Persistence
{
    public static class DependencyInjectionExtension
    {
        public static IServiceCollection AddInfrastructurePersistenceServices(this IServiceCollection services)
        {
            services.AddDbContext<CinemaContext>(options =>
            {
                options.UseInMemoryDatabase("CinemaDb")
                    .EnableSensitiveDataLogging()
                    .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });

            services.AddTransient<IShowtimesRepository, ShowtimesRepository>();
            services.AddTransient<IMovieServiceWrapper, MovieServiceWrapper>();
            services.AddTransient<ITicketsRepository, TicketsRepository>();
            services.AddTransient<IAuditoriumsRepository, AuditoriumsRepository>();

            return services;
        }
    }
}