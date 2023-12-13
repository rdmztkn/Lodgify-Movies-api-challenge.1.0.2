using ApiApplication.Core.Application.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace ApiApplication.Infrastructure.Cache
{
    public static class DependencyInjectionExtension
    {
        public static IServiceCollection AddInfrastructureCacheServices(this IServiceCollection services, IConfiguration configuration)
        {
            var host = configuration["Redis:Host"];
            var port = configuration["Redis:Port"];

            var opt = new ConfigurationOptions();
            opt.EndPoints.Add($"{host}:{port}");

            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(opt));
            services.AddTransient<ICacheRepository, CacheRepository>();



            return services;
        }
    }
}