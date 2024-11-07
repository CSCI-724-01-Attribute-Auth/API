using API.Authorization;
using API.Services.Implementations;
using API.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace API.App_Start
{
    internal static class RegisterServiceConfig
    {
        internal static void RegisterDependency(this IServiceCollection services)
        {
            services.AddScoped<IMovieService, MovieService>();
            services.AddScoped<IPersonService, PersonService>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<DataFactory>();
            services.AddScoped<IndexBuilder>();
            services.AddSingleton<IndexCache>();
            services.AddSingleton<Mapper>();
            services.AddSingleton<ResponseBuilder>();
            services.AddSingleton<Retriever>();
            services.AddHostedService<CacheRefreshService>();
        }
    }
}
