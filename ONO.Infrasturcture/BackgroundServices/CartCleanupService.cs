using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ONO.Core.Entities;
using ONO.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Infrasturcture.BackgroundServices
{
    public class CartCleanupService : BackgroundService
    {
        readonly IServiceProvider _serviceProvider;

        public CartCleanupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var cartService = scope.ServiceProvider.GetRequiredService<ICartService>();

                    DateTime time = DateTime.Now.AddDays(-1);

                    await cartService.CleanupExpiredCarts(time);
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
