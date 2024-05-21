using NmhAssignment.Models;
using NmhAssignment.Services;
using KeyValueWebApi.Services;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace NmhAssignment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalculationController : ControllerBase
    {
        private readonly KeyValueStorage keyValueStorage;
        private readonly IConnection rabbitMqConnection;

        public CalculationController(
            KeyValueStorage keyValueStorage,
            IConnection rabbitMqConnection)
        {
            this.keyValueStorage = keyValueStorage;
            this.rabbitMqConnection = rabbitMqConnection;
        }

        [HttpPost("{key:int}")]
        public IActionResult Calculation(int key, [FromBody] CalculationInput input)
        {
            if (input == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            decimal previousValue = 0;

            // Caclulate and set value of entry in key value cache
            if (keyValueStorage.Storage.TryGetValue(key, out var entry))
            {
                previousValue = keyValueStorage.Storage[key].Value;

                if ((DateTime.UtcNow - entry.LastUpdated).TotalSeconds > 15)
                {
                    keyValueStorage.Storage[key] = new KeyValueModel(2, DateTime.UtcNow);
                }
                else
                {
                    try
                    {
                        var log = Math.Log((double)(input.Input / entry.Value));
                        var newValue = (decimal)Math.Pow(log, 1.0 / 3.0);

                        // Should we update LastSeen value as well? not specified in requirements therefore updating
                        keyValueStorage.Storage[key] = new KeyValueModel(newValue, DateTime.UtcNow);
                    }
                    catch (Exception)
                    {
                        return BadRequest(error: $"Not possible to calculate a new value from input value of {input.Input}.");
                    }
                }
            }
            else
            {
                keyValueStorage.Storage[key] = new KeyValueModel(2, DateTime.UtcNow);
            }

            var result = new CalculationResponse
            {
                computed_value = keyValueStorage.Storage[key].Value,
                input_value = input.Input,
                previous_value = previousValue
            };

            // Send result of calculation to RabitMQ queue
            using var channel = rabbitMqConnection.CreateModel();

            channel.QueueDeclare(queue: "calculation",
                      durable: false,
                      exclusive: false,
                      autoDelete: false,
                      arguments: null);

            var message = JsonSerializer.Serialize(result);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: string.Empty, routingKey: "calculation", basicProperties: null, body: body);

            return Ok(result);
        }
    }
}