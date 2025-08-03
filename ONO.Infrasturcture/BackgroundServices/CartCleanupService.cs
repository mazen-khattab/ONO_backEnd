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
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[CartCleanupService] Running at {DateTime.Now}");
                Console.ResetColor();

                using (var scope = _serviceProvider.CreateScope())
                {
                    var cartService = scope.ServiceProvider.GetRequiredService<ICartService>();

                    await cartService.CleanupExpiredCarts();
                }

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
