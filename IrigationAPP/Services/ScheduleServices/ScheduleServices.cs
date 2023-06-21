using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using IrigationAPP.Data;
using IrigationAPP.Models;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Client.Options;

namespace IrigationAPP.Services.ScheduleServices
{
    public class ScheduleService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ScheduleService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var currentTime = DateTime.Now;
                    var currentDayOfWeek = (int)currentTime.DayOfWeek;
                    var currentTimeTimeOfDay = currentTime.TimeOfDay;

                    var allSchedules = await context.Schedule.ToListAsync();

                    foreach (var schedule in allSchedules.Where(s => s.Status == null))
                    {
                        schedule.Status = "Waiting";
                    }

                    var schedulesToStart = allSchedules
                        .Where(s => s.StartTime.TimeOfDay <= currentTimeTimeOfDay &&
                                    s.Status == "Waiting" &&
                                    (int)s.StartTime.DayOfWeek == currentDayOfWeek);

                    foreach (var schedule in schedulesToStart)
                    {
                        schedule.Status = "In Progress";

                        var valveState = new ValveState { ValveId = 1, State = 1, Time = DateTime.Now };
                        context.ValveState.Add(valveState);
                        await PublishAsync("/valve/status", "1");
                    }

                    var schedulesToStop = allSchedules
                        .Where(s => s.StopTime.TimeOfDay <= currentTimeTimeOfDay &&
                                    s.Status == "In Progress");

                    foreach (var schedule in schedulesToStop)
                    {
                        schedule.Status = "Finished";

                        var valveState = new ValveState { ValveId = 1, State = 0, Time = DateTime.Now };
                        context.ValveState.Add(valveState);
                        await PublishAsync("/valve/status", "0");
                    }

                    await context.SaveChangesAsync();
                }

                await Task.Delay(6000, stoppingToken); 
            }
        }
        private async Task PublishAsync(string topic, string payload)
        {
            try
            {
                var factory = new MqttFactory();
                var mqttClient = factory.CreateMqttClient();

                var options = new MqttClientOptionsBuilder()
                    .WithClientId(Guid.NewGuid().ToString())
                    .WithTcpServer("broker.emqx.io")
                    .WithCleanSession()
                    .Build();

                await mqttClient.ConnectAsync(options, CancellationToken.None);

                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(payload)
                    .WithExactlyOnceQoS()
                    .WithRetainFlag(false)
                    .Build();

                await mqttClient.PublishAsync(message, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in PublishAsync: {ex.Message}");
            }
        }
    }
}




