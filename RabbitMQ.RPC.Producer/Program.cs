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
            var list = new List<string>();

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            
            var replyTo = "AcknowledgeRPC";
            var msgQueue = "MessageRPC";

            channel.QueueDeclare(queue: msgQueue,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.QueueDeclare(queue: replyTo,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            string message = "rpc";
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            while (true)
            {
                var props = channel.CreateBasicProperties();
                props.Persistent = true;

                var correlatinoId = Guid.NewGuid().ToString();
                list.Add(correlatinoId);
                props.CorrelationId = correlatinoId;
                props.ReplyTo = replyTo;

                channel.BasicPublish(exchange: "",
                             routingKey: msgQueue,
                             basicProperties: props,
                             body: body);
                
                var consumer = new EventingBasicConsumer(channel);

                channel.BasicConsume(
                            consumer: consumer,
                            queue: props.ReplyTo,
                            autoAck: true);

                consumer.Received += (sender, e) =>
                {
                    var answer = e.Body.ToArray();
                    if (list.Contains(e.BasicProperties.CorrelationId.ToString()))
                    {
                        Console.WriteLine($"Message: {Encoding.UTF8.GetString(answer)}");
                        Console.WriteLine($"Count: {list.Count}");
                        list.Remove(e.BasicProperties.CorrelationId);
                        Console.WriteLine($"Count: {list.Count}");
                    }
                };

                Thread.Sleep(2000);
            }
        }
    }
}
