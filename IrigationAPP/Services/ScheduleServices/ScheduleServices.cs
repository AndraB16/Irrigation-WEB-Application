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

                    // Get current day of the week and time of day
                    var currentTime = DateTime.Now;
                    var currentDayOfWeek = (int)currentTime.DayOfWeek;
                    var currentTimeTimeOfDay = currentTime.TimeOfDay;

                    // Get all schedules
                    var allSchedules = await context.Schedule.ToListAsync();

                    // Set status to "Waiting" if it is null
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
                        // Change status to "In Progress"
                        schedule.Status = "In Progress";

                        // Add record to ValveState
                        var valveState = new ValveState { ValveId = 1, State = 1, Time = DateTime.Now };
                        context.ValveState.Add(valveState);
                        // Publish MQTT message
                        await PublishAsync("/andra/licenta/irig/in", "1");
                    }

                    var schedulesToStop = allSchedules
                        .Where(s => s.StopTime.TimeOfDay <= currentTimeTimeOfDay &&
                                    s.Status == "In Progress");

                    foreach (var schedule in schedulesToStop)
                    {
                        // Change status to "Finished"
                        schedule.Status = "Finished";

                        // Add record to ValveState
                        var valveState = new ValveState { ValveId = 1, State = 0, Time = DateTime.Now };
                        context.ValveState.Add(valveState);
                        await PublishAsync("/andra/licenta/irig/in", "0");
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

/*using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using IrigationAPP.Data;
using IrigationAPP.Models;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Client.Publishing;
using System.Text;

namespace IrigationAPP.ScheduleServices
{
    public class ScheduleService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IMqttClient _mqttClient;

        public ScheduleService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;

            // Create MQTT client
            var mqttFactory = new MqttFactory();
            _mqttClient = mqttFactory.CreateMqttClient();
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    try
                    {
                        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                        // Get current time
                        var currentTime = DateTime.Now;

                        // Get all schedules
                        var allSchedules = await context.Schedule.ToListAsync();

                        var schedulesToStart = allSchedules
                            .Where(s => s.StartTime <= currentTime &&
                                        s.Status == "Waiting");

                        foreach (var schedule in schedulesToStart)
                        {
                            // Change status to "In Progress"
                            schedule.Status = "In Progress";

                            // Write to ValveState table
                            var valveState = new ValveState { ValveId = 1, State = 1, Time = DateTime.Now };
                            context.ValveState.Add(valveState);
                        }

                        var schedulesToStop = allSchedules
                            .Where(s => s.StopTime <= currentTime &&
                                        s.Status == "In Progress");

                        foreach (var schedule in schedulesToStop)
                        {
                            // Change status to "Finished"
                            schedule.Status = "Finished";

                            // Write to ValveState table
                            var valveState = new ValveState { ValveId = 1, State = 0, Time = DateTime.Now };
                            context.ValveState.Add(valveState);
                        }

                        await context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception: {ex.Message}");
                    }
                }

                await Task.Delay(60000, stoppingToken); // Check every minute
            }
        }


        /* protected override async Task ExecuteAsync(CancellationToken stoppingToken)
         {
             // Create MQTT client options
             var mqttOptions = new MqttClientOptionsBuilder()
                 .WithTcpServer("broker.emqx.io")
                 .Build();

             // Connect to MQTT broker
             //await _mqttClient.ConnectAsync(mqttOptions, stoppingToken);
             var factory = new MqttFactory();
             var _mqttClient = factory.CreateMqttClient();

             try
             {
                 await _mqttClient.ConnectAsync(mqttOptions, stoppingToken);
             }
             catch (Exception ex)
             {
                 Console.WriteLine($"Error connecting to MQTT broker: {ex.Message}");
             }


             while (!stoppingToken.IsCancellationRequested)
             {
                 using (var scope = _serviceScopeFactory.CreateScope())
                 {
                     var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                     // Get current day of the week and time of day
                     var currentTime = DateTime.Now;
                     var currentDayOfWeek = (int)currentTime.DayOfWeek;
                     var currentTimeTimeOfDay = currentTime.TimeOfDay;

                     // Get all schedules
                     var allSchedules = await context.Schedule.ToListAsync();

                     var schedulesToStart = allSchedules
                         .Where(s => s.StartTime.TimeOfDay <= currentTimeTimeOfDay &&
                                     s.Status == "Waiting" &&
                                     (int)s.StartTime.DayOfWeek == currentDayOfWeek);

                     foreach (var schedule in schedulesToStart)
                     {
                         // Change status to "In Progress"
                         schedule.Status = "In Progress";

                         // Write to ValveState table
                         var valveState = new ValveState { ValveId = 1, State = 1, Time = DateTime.Now };
                         context.ValveState.Add(valveState);

                         // Publish MQTT message
                         var message = new MqttApplicationMessageBuilder()
                             .WithTopic("/andra/licenta/irig/in")
                             .WithPayload("1")
                             .WithAtMostOnceQoS()
                             .Build();

                         //await _mqttClient.PublishAsync(message, stoppingToken);
                         try
                         {
                             await _mqttClient.PublishAsync(message, stoppingToken);
                         }
                         catch (Exception ex)
                         {
                             Console.WriteLine($"Error publishing MQTT message: {ex.Message}");
                         }
                     }

                     var schedulesToStop = allSchedules
                         .Where(s => s.StopTime.TimeOfDay <= currentTimeTimeOfDay &&
                                     s.Status == "In Progress");

                     foreach (var schedule in schedulesToStop)
                     {
                         // Change status to "Finished"
                         schedule.Status = "Finished";

                         // Write to ValveState table
                         var valveState = new ValveState { ValveId = 1, State = 0, Time = DateTime.Now };
                         context.ValveState.Add(valveState);

                         // Publish MQTT message
                         var message = new MqttApplicationMessageBuilder()
                             .WithTopic("/andra/licenta/irig/in")
                             .WithPayload("0")
                             .WithAtMostOnceQoS()
                             .Build();

                         await _mqttClient.PublishAsync(message, stoppingToken);
                     }

                     await context.SaveChangesAsync();
                 }

                 await Task.Delay(6000, stoppingToken); // Check every minute
             }
         }
    }
}*/


