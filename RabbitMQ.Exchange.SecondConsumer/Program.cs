using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RabbitMQ.Exchange.SecondConsumer
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            ConsumeMessage();
        }

        private static void ConsumeMessage()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Message: {message}");
            };

            channel.BasicConsume(queue: "first-queue",
                                 autoAck: true, //!!!!!!!!!!!!!!!!!!!!!!!!!!!
                                 consumer: consumer);

            Console.ReadLine();
        }

        private static void ConsumeMessageFromFanoutExchange()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var queue = "fanoutQueueSecond";
            var exchange = "fanoutExchange";
            var routingKey = "top secret";
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(queue: queue, true, false, false, null);

            var header = new Dictionary<string, object>
            {
                { "tutaj", "heder"},
                { "jest", "bro"}
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

        private static void ConsumeMessageFromHeaderExchange()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var queue = "headerQueueSecond";
            var exchange = "headerExchange";
            var routingKey = "rawrrr";
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(queue: queue, true, false, false, null);

            var header = new Dictionary<string, object>
            {
                { "get", "get"},
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

        private static void ConsumeMessageFromTopicExchange()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var queue = "topicQueueSecond";
            var exchange = "topicExchange";
            var routingKey = "topic.add";
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
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
