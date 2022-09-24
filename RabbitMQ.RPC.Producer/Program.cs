using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RabbitMQ.RPC.Producer
{
    class Program
    {
        public static void Main()
        {

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            var consumer = new EventingBasicConsumer(channel);

            channel.QueueDeclare(queue: "MessageRPC",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.QueueDeclare(queue: "AcknowledgeRRPC",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var props = channel.CreateBasicProperties();
            props.Persistent = true;

            props.CorrelationId = Guid.NewGuid().ToString();
            props.ReplyTo = "AcknowledgeRRPC";

            string message = "rpc";
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            while (true)
            {
                channel.BasicPublish(exchange: "",
                             routingKey: "MessageRPC",
                             basicProperties: props,
                             body: body);

                //channel.BasicConsume(
                //            consumer: consumer,
                //            queue: props.ReplyTo,
                //            autoAck: true);

                Thread.Sleep(2000);
            }
        }
    }
}
