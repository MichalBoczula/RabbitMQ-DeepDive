using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Exchange.Consumer
{
    internal class HeaderConsumer
    {
        public static void ConsumeMessageFromTopicExchange()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var queue = "headerQueueFirst";
            var exchange = "headerExchange";
            var routingKey = string.Empty;

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: queue, true, false, false, null);

            var header = new Dictionary<string, object>
            {
                { "add", "add"},
                { "update", "update"},
            };
            channel.QueueBind(queue, exchange, routingKey, header);

            var props = channel.CreateBasicProperties();
            props.Persistent = true;
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                //e.BasicProperties.Headers.Keys.Select(x => x).ToList().ForEach(x => Console.WriteLine(x));
                Console.WriteLine($"Message: {message}");
            };

            channel.BasicConsume(queue: queue,
                                    autoAck: true,
                                    consumer: consumer);
            Console.ReadLine();
        }
    }
}
