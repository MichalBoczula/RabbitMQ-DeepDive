using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RabbitMQ.RPC.Producer
{
    class Program
    {
        /// <summary>
        /// Standard RPC
        /// </summary>
        //public static void Main()
        //{
        //    var list = new List<string>();

        //    var factory = new ConnectionFactory() { HostName = "localhost" };
        //    using var connection = factory.CreateConnection();
        //    using var channel = connection.CreateModel();

        //    var replyTo = "AcknowledgeRPC";
        //    var msgQueue = "MessageRPC";

        //    channel.QueueDeclare(queue: msgQueue,
        //                         durable: true,
        //                         exclusive: false,
        //                         autoDelete: false,
        //                         arguments: null);

        //    channel.QueueDeclare(queue: replyTo,
        //                         durable: true,
        //                         exclusive: false,
        //                         autoDelete: false,
        //                         arguments: null);

        //    string message = "rpc";
        //    var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

        //    while (true)
        //    {
        //        var props = channel.CreateBasicProperties();
        //        props.Persistent = true;

        //        var correlatinoId = Guid.NewGuid().ToString();
        //        list.Add(correlatinoId);
        //        props.CorrelationId = correlatinoId;
        //        props.ReplyTo = replyTo;

        //        channel.BasicPublish(exchange: "",
        //                     routingKey: msgQueue,
        //                     basicProperties: props,
        //                     body: body);

        //        var consumer = new EventingBasicConsumer(channel);

        //        channel.BasicConsume(
        //                    consumer: consumer,
        //                    queue: props.ReplyTo,
        //                    autoAck: true);

        //        consumer.Received += (sender, e) =>
        //        {
        //            var answer = e.Body.ToArray();
        //            if (list.Contains(e.BasicProperties.CorrelationId.ToString()))
        //            {
        //                Console.WriteLine($"Message: {Encoding.UTF8.GetString(answer)}");
        //                Console.WriteLine($"Count: {list.Count}");
        //                list.Remove(e.BasicProperties.CorrelationId);
        //                Console.WriteLine($"Count: {list.Count}");
        //            }
        //        };

        //        Thread.Sleep(2000);
        //    }
        //}

        /// <summary>
        /// RPC with state machine
        /// </summary>
        public static void Main()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var exchange = "RPCExchange";

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange, ExchangeType.Headers, true, false, null);

            var message = new { Name = "RPC", Message = "RPC" };
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            while (true)
            {
                var props = channel.CreateBasicProperties();
                props.Persistent = true;
                var correlatinoId = Guid.NewGuid().ToString();
                props.CorrelationId = correlatinoId;
                props.ReplyTo = "StateRPC";

                props.Headers = new Dictionary<string, object>
                {
                    { "msg", "msg" },
                };

                channel.BasicPublish(exchange: exchange,
                             routingKey: string.Empty,
                             basicProperties: props,
                             body: body);

                props.Headers = new Dictionary<string, object>
                {
                    { "correlation", "correlation" }
                };

                var flag = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(false));

                channel.BasicPublish(exchange: exchange,
                             routingKey: string.Empty,
                             basicProperties: props,
                             body: flag);

                Thread.Sleep(2000);
            }
        }
    }
}
