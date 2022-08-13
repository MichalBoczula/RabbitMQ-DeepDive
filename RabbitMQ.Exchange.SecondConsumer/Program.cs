using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQ.Exchange.SecondConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var queue = "topicQueueSecond";
            var exchange = "topicExchange";
            var routingKey = "topic.add";

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: queue, true, false, false, null);
            channel.QueueBind(queue, exchange, routingKey);

            var props = channel.CreateBasicProperties();
            props.Persistent = true;
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Message: {message} {e.RoutingKey}");
            };

            channel.BasicConsume(queue: queue,
                                    autoAck: true,
                                    consumer: consumer);
            Console.ReadLine();
        }
    }
}
