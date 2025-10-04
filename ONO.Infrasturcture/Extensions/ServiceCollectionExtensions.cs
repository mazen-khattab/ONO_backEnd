using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ONO.Application.Decorator.CacheDecorator;
using ONO.Application.Interfaces;
using ONO.Application.Mappers;
using ONO.Application.Services;
using ONO.Core.Entities;
using ONO.Core.Interfaces;
using ONO.Infrasturcture.BackgroundServices;
using ONO.Infrasturcture.Persistence;
using ONO.Infrasturcture.Repositories;

namespace ONO.Infrasturcture.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

            // services and ropes registration
            services.AddScoped(typeof(IRepo<>), typeof(Repo<>));
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddScoped(typeof(IServices<>), typeof(Services<>));
            services.AddScoped(typeof(IProductServices), typeof(ProductServices));
            services.AddScoped(typeof(IAuthServices), typeof(AuthServices));
            services.AddScoped(typeof(IOrderRepo), typeof(OrderRepo));
            services.AddScoped(typeof(IUserServices), typeof(UserServices));
            services.AddScoped(typeof(IOrderServices), typeof(OrderServices));
            services.AddScoped(typeof(ICartService), typeof(CartServices));

            // cache and decorator registration
            services.AddMemoryCache();
            services.AddSingleton(typeof(ICacheServices), typeof(CacheServices));
            services.Decorate(typeof(IProductServices), typeof(CacheProductDecorator));

            services.AddAutoMapper(typeof(MappingConfig));

            services.AddHostedService<CartCleanupService>();

            return services;
        }
    }
}
