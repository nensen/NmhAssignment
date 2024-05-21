using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Assignment.Services
{
    public class RabbitMqConsumerService : BackgroundService
    {
        private readonly IConnection rabbitMqConnection;

        public RabbitMqConsumerService(IConnection rabbitMqConnection)
        {
            this.rabbitMqConnection = rabbitMqConnection;
        }

        /// <summary>
        /// Read messages from rabbitMQ and write them to the console
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var channel = rabbitMqConnection.CreateModel();
           
            var consumer = new EventingBasicConsumer(channel);
          
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(message);
            };

            channel.BasicConsume(queue: "calculation",
                     autoAck: true,
                     consumer: consumer);

            return Task.CompletedTask;
        }
    }
}