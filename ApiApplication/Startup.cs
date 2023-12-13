using ApiApplication.Core.Application;
using ApiApplication.Core.Application.Repositories;
using ApiApplication.Core.Application.Services;
using ApiApplication.Infrastructure.Cache;
using ApiApplication.Infrastructure.Infrastructure.Persistence;
using ApiApplication.Infrastructure.Infrastructure.Persistence.Repositories;
using ApiApplication.Infrastructure.Persistence;
using ApiApplication.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace ApiApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ExceptionHandlingMiddleware>();
            services.AddSingleton<RequestTimeLoggingMiddleware>();

            services.ConfigureApplicationServices();
            services.AddInfrastructureCacheServices(Configuration);
            services.AddInfrastructurePersistenceServices();

            services.AddControllers();
            services.AddHttpClient();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            // make sure it's added before other middleware as it's going to wait for the response
            app.UseMiddleware<RequestTimeLoggingMiddleware>();
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            SampleData.Initialize(app);
        }
    }
}