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
                    // Assuming the message payload is a string
                    var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                    // Debug: Print the received message
                    Console.WriteLine($"Received message: {payload}");

                    var parts = payload.Split(',');
                    var collectorId = int.Parse(parts[0]); // Parse collector ID
                    var soilMoisturePercent = int.Parse(parts[1]);

                    var dataRead = new DataRead // Change this to your model class name
                    {
                        collectorId = collectorId,
                        soilMoisturePercent = soilMoisturePercent,
                        time = DateTime.UtcNow
                    };

                    // Use IServiceScopeFactory to create a new scope
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        context.DataRead.Add(dataRead); // Change this to your DbSet name
                        context.SaveChanges();

                        // Debug: Print a message after saving changes
                        Console.WriteLine($"Saved data to database: {dataRead}");
                    }
                }
                catch (Exception ex)
                {
                    // Debug: Print any exceptions
                    Console.WriteLine($"Exception while handling message: {ex}");
                }
            });
        }
    }
}

