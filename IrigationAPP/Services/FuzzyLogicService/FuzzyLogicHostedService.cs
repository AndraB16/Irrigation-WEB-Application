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
            var factory = new MqttFactory();
            mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("broker.emqx.io", 1883) 
                .Build();

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

                    var message = new MqttApplicationMessageBuilder()
                        .WithTopic("/valve/status")
                        .WithPayload(newEntry.State.ToString())
                        .WithExactlyOnceQoS()
                        .WithRetainFlag()
                        .Build();

                    await mqttClient.PublishAsync(message);

                    await dbContext.SaveChangesAsync();

                    await Task.Delay((int)irrigationTime * 60 * 1000); 

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

                    await Task.Delay(10800000, stoppingToken); // așteptăm 3 ore
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await mqttClient.DisconnectAsync();
        }
    }
}




