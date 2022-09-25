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
            channel.BasicQos(0, 1, true);
            var msgQueue = "MessageRPC";

            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var replyTo = e.BasicProperties.ReplyTo;
                var correlationId = e.BasicProperties.CorrelationId;
                var props = channel.CreateBasicProperties();
                props.CorrelationId = correlationId;

                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Message: {message}, ReplyTo: {replyTo}, CorrelationId: {correlationId}");

                channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);

                channel.BasicPublish(exchange: "",
                             routingKey: replyTo,
                             basicProperties: props,
                             body: body);
            };

            channel.BasicConsume(queue: msgQueue,
                                 autoAck: false, 
                                 consumer: consumer);


            Console.ReadLine();
        }
    }
}
