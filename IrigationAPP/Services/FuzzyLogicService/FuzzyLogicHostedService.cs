/*using IrigationAPP.Data;
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

                    var newIrrigationTime = new IrrigationTime
                    {
                        time = DateTime.Now, 
                        irrigationTime = irrigationTime
                    };

                    dbContext.IrrigationTime.Add(newIrrigationTime);
                    await dbContext.SaveChangesAsync();
                }

                await Task.Delay(300000, stoppingToken);  
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {

            return Task.CompletedTask;
        }
    }

}*/

using IrigationAPP.Data;
using IrigationAPP.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IrigationAPP.Services.FuzzyLogicService
{
    public class FuzzyLogicHostedService : IHostedService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IMqttClient mqttClient;

        public FuzzyLogicHostedService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Crearea unui nou client MQTT
            var factory = new MqttFactory();
            mqttClient = factory.CreateMqttClient();

            // Configurarea opțiunilor clientului MQTT
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("broker.emqx.io", 1883) // Portul 1883 este portul standard pentru MQTT
                .Build();

            // Conectarea la broker
            await mqttClient.ConnectAsync(options);

            _ = ExecuteAsync(cancellationToken);
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

                    var newIrrigationTime = new IrrigationTime
                    {
                        time = DateTime.Now,
                        irrigationTime = irrigationTime
                    };

                    dbContext.IrrigationTime.Add(newIrrigationTime);

                    var newEntry = new ValveState
                    {
                        State = 1,
                        Time = DateTime.Now,
                        ValveId = 1
                    };

                    dbContext.ValveState.Add(newEntry);

                    // Crearea unui nou mesaj MQTT
                    var message = new MqttApplicationMessageBuilder()
                        .WithTopic("/valve/status")
                        .WithPayload(newEntry.State.ToString())
                        .WithExactlyOnceQoS()
                        .WithRetainFlag()
                        .Build();

                    // Publicarea mesajului
                    await mqttClient.PublishAsync(message);

                    await dbContext.SaveChangesAsync();

                    await Task.Delay((int)irrigationTime * 60 * 1000); // așteptăm timpul de irigație, convertit în milisecunde

                    newEntry = new ValveState
                    {
                        State = 0,
                        Time = DateTime.Now,
                        ValveId = 1
                    };

                    dbContext.ValveState.Add(newEntry);

                    message = new MqttApplicationMessageBuilder()
                        .WithTopic("/valve/status")
                        .WithPayload(newEntry.State.ToString())
                        .WithExactlyOnceQoS()
                        .WithRetainFlag()
                        .Build();

                    await mqttClient.PublishAsync(message);

                    await dbContext.SaveChangesAsync();

                    await Task.Delay(300000, stoppingToken); // așteptăm 5 minute înainte de a începe din nou
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // Deconectarea de la broker
            await mqttClient.DisconnectAsync();
        }
    }
}




