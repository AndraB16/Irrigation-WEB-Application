using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IrigationAPP.AccuWeatherServices
{
    public class WeatherUpdateService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer _timer;

        public WeatherUpdateService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(1));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var accuWeatherService = scope.ServiceProvider.GetRequiredService<AccuWeatherService>();
                accuWeatherService.FetchDataAsync().Wait();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }


    /* public class WeatherUpdateService : IHostedService
     {
         private readonly IServiceScopeFactory _scopeFactory;

         public WeatherUpdateService(IServiceScopeFactory scopeFactory)
         {
             _scopeFactory = scopeFactory;
         }

         public async Task StartAsync(CancellationToken cancellationToken)
         {
             using (var scope = _scopeFactory.CreateScope())
             {
                 var accuWeatherService = scope.ServiceProvider.GetRequiredService<AccuWeatherService>();
                 await accuWeatherService.FetchDataAsync();
             }
         }

         public Task StopAsync(CancellationToken cancellationToken)
         {
             // Handle when your service is stopping
             return Task.CompletedTask;
         }
     }*/

}
