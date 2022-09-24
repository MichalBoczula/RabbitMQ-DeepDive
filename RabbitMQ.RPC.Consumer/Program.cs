using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace RabbitMQ.RPC.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var replyTo = e.BasicProperties.ReplyTo;
                var correlationId = e.BasicProperties.CorrelationId;

                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Message: {message} {replyTo} {correlationId}");

                channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(queue: "MessageRPC",
                                 autoAck: false, 
                                 consumer: consumer);

            Console.ReadLine();
        }
    }
}
