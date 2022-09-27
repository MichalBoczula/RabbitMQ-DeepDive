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
        /// <summary>
        /// Standard RPC 
        /// </summary>
        /// <param name="args"></param>
        //static void Main(string[] args)
        //{
        //    var factory = new ConnectionFactory() { HostName = "localhost" };
        //    using var connection = factory.CreateConnection();
        //    using var channel = connection.CreateModel();
        //    var consumer = new EventingBasicConsumer(channel);
        //    channel.BasicQos(0, 1, true);
        //    var msgQueue = "MessageRPC";

        //    //int i = 0;
        //    consumer.Received += (sender, e) =>
        //    {
        //        var body = e.Body.ToArray();
        //        var replyTo = e.BasicProperties.ReplyTo;
        //        var correlationId = e.BasicProperties.CorrelationId;
        //        var props = channel.CreateBasicProperties();
        //        props.CorrelationId = correlationId;

        //        var message = Encoding.UTF8.GetString(body);
        //        Console.WriteLine($"Message: {message}, ReplyTo: {replyTo}, CorrelationId: {correlationId}");

        //        //if (i == 5)
        //        //{
        //        //    throw new Exception();
        //        //};
        //        //i++;
        //        channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);

        //        var answer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject("ACK"));

        //        channel.BasicPublish(exchange: "",
        //                     routingKey: replyTo,
        //                     basicProperties: props,
        //                     body: answer);
        //    };

        //    channel.BasicConsume(queue: msgQueue,
        //                         autoAck: false,
        //                         consumer: consumer);

        //    Console.ReadLine();
        //}

        /// <summary>
        /// RPC with state machine
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var queue = "MessageRPC";
            var exchange = "RPCExchange";
            var exchangeAck = "RpcExchangeAck";
            var routingKey = string.Empty;

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: queue, true, false, false, null);
            channel.BasicQos(0, 1, true);
            channel.ExchangeDeclare(exchangeAck, ExchangeType.Headers, true, false, null);

            var header = new Dictionary<string, object>
            {
                { "msg", "msg"},
            };
            channel.QueueBind(queue, exchange, routingKey, header);

            var props = channel.CreateBasicProperties();
            props.Persistent = true;
            props.Headers = new Dictionary<string, object>
            {
                { "correlationAck", "correlationAck" },
            };
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Message: {message} {e.BasicProperties.CorrelationId}");

                Thread.Sleep(1000);
                channel.BasicAck(e.DeliveryTag, multiple: false);
                var id = Encoding.UTF8.GetBytes(e.BasicProperties.CorrelationId);
                channel.BasicPublish(exchange: exchangeAck,
                             routingKey: string.Empty,
                             basicProperties: props,
                             body: id);
            };

            channel.BasicConsume(queue: queue,
                                    autoAck: false,
                                    consumer: consumer);
            Console.ReadLine();
        }
    }
}
