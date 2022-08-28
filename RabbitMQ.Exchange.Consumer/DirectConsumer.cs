using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Exchange.Consumer
{
    public static class DirectConsumer
    {
        public static void ConsumeMessageFromDiectQueue()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var queue = "directQueue";

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Message: {message}");

                channel.BasicConsume(queue: queue,
                                     autoAck: true,
                                     consumer: consumer);
            };

            Console.ReadLine();
        }
    }
}
