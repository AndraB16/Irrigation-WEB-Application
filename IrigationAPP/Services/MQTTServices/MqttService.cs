using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using System.Threading.Tasks;
using IrigationAPP.Data;
using IrigationAPP.Models;
using System;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace IrigationAPP.MQTTServices
{
    public class MqttService
    {
        private readonly IManagedMqttClient _mqttClient;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public MqttService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _mqttClient = new MqttFactory().CreateManagedMqttClient();
        }

        public async Task StartAsync()
        {
            var options = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithClientId(Guid.NewGuid().ToString())
                    .WithTcpServer("broker.emqx.io")
                    .Build())
                .Build();

            await _mqttClient.StartAsync(options);
            await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("/andra/licenta/irig/out").Build());

            _mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                try
                {
                    var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                    Console.WriteLine($"Received message: {payload}");

                    var parts = payload.Split(',');
                    var collectorId = int.Parse(parts[0]);
                    var soilMoisturePercent = int.Parse(parts[1]);

                    var dataRead = new DataRead 
                    {
                        collectorId = collectorId,
                        soilMoisturePercent = soilMoisturePercent,
                        time = DateTime.Now
                    };

                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        context.DataRead.Add(dataRead);
                        context.SaveChanges();

                        Console.WriteLine($"Saved data to database: {dataRead}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception while handling message: {ex}");
                }
            });
        }
    }
}

