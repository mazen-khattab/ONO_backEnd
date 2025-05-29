using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ONO.Infrasturcture.Persistence;
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

            return services;
        }
    }
}
