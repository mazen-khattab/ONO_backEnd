using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ONO.Application.Interfaces;
using ONO.Application.Services;
using ONO.Core.Interfaces;
using ONO.Infrasturcture.Mappers;
using ONO.Infrasturcture.Persistence;
using ONO.Infrasturcture.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Infrasturcture.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string connectionString)
        {
            // Register DbContext
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
            services.AddScoped(typeof(IRepo<>), typeof(Repo<>));
            services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            services.AddScoped(typeof(IServices<>), typeof(Services<>));
            services.AddScoped(typeof(IAuthServices), typeof(AuthServices));

            services.AddAutoMapper(typeof(MappingConfig));

            return services;
        }
    }
}
