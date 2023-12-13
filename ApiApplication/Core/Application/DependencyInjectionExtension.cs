using ApiApplication.Core.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Reflection;

namespace ApiApplication.Core.Application
{
    public static class DependencyInjectionExtension
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddAutoMapper(assembly);
            services.AddMediatR(opt => opt.RegisterServicesFromAssemblies(assembly));

            services.AddSingleton<ApiClientGrpc>();
            services.AddScoped<MovieServiceWrapper>();

            return services;
        }
    }
}