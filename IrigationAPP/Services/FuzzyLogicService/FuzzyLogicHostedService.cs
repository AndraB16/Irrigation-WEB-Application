using IrigationAPP.Data;
using IrigationAPP.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IrigationAPP.Services.FuzzyLogicService
{
    public class FuzzyLogicHostedService : IHostedService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public FuzzyLogicHostedService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _ = ExecuteAsync(cancellationToken);

            return Task.CompletedTask;
        }

        protected async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var fuzzyLogicService = scope.ServiceProvider.GetRequiredService<FuzzyLogicServices>();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var irrigationTime = await fuzzyLogicService.DetermineIrigationTime();

                    // Crearea unui nou obiect IrrigationTime
                    var newIrrigationTime = new IrrigationTime
                    {
                        time = DateTime.Now, 
                        irrigationTime = irrigationTime
                    };

                    // Adăugarea noului obiect în DbSet și salvarea modificărilor
                    dbContext.IrrigationTime.Add(newIrrigationTime);
                    await dbContext.SaveChangesAsync();
                }

                await Task.Delay(300000, stoppingToken);  // Așteaptă o oră înainte de a repeta
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Implementare opțională, dacă este necesar.
            return Task.CompletedTask;
        }
    }

}


