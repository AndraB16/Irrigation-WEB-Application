using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IrigationAPP.Data;
using IrigationAPP.Models;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Client;
using System.Text;
using System.Threading;

namespace IrigationAPP.Controllers
{
    public class ValveStatesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ValveStatesController(ApplicationDbContext context)
        {
            _context = context;
        }
       
        public async Task<IActionResult> Index()
        {
            var allStates = await _context.ValveState.ToListAsync();

            var latestStates = allStates
                .GroupBy(v => v.ValveId)
                .Select(g => g.OrderByDescending(v => v.Time).First())
                .OrderBy(v => v.ValveId)
                .ToList();

            return View(latestStates);
        }
        
        [HttpPost]
        public async Task<IActionResult> TurnOn(int id)
        {
            var newValveState = new ValveState { ValveId = id, State = 1, Time = DateTime.Now };

            _context.ValveState.Add(newValveState);
            await _context.SaveChangesAsync();

            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithClientId(Guid.NewGuid().ToString())
                .WithTcpServer("broker.emqx.io")
                .Build();

            await mqttClient.ConnectAsync(options, CancellationToken.None);

            string topic = "/valve/status";

            string payload = newValveState.State.ToString();

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithExactlyOnceQoS()
                .Build();

            await mqttClient.PublishAsync(message, CancellationToken.None);

            await mqttClient.DisconnectAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> TurnOff(int id)
        {
            var newValveState = new ValveState { ValveId = id, State = 0, Time = DateTime.Now };
            _context.ValveState.Add(newValveState);
            await _context.SaveChangesAsync();

            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithClientId(Guid.NewGuid().ToString())
                .WithTcpServer("broker.emqx.io")
                .Build();

            await mqttClient.ConnectAsync(options, CancellationToken.None);

            string topic = "/valve/status";

            string payload = newValveState.State.ToString();

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithExactlyOnceQoS()
                .Build();

            await mqttClient.PublishAsync(message, CancellationToken.None);

            await mqttClient.DisconnectAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ValveStateExists(int id)
        {
            return _context.ValveState.Any(e => e.Id == id);
        }

    }
}
