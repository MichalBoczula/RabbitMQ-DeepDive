﻿using Newtonsoft.Json;
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
            var queue = "MessageRPC";
            var exchange = "RPCExchange";
            var routingKey = string.Empty;

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: queue, true, false, false, null);

            var header = new Dictionary<string, object>
            {
                { "msg", "msg"},
            };
            channel.QueueBind(queue, exchange, routingKey, header);

            var props = channel.CreateBasicProperties();
            props.Persistent = true;
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Message: {message}");
            };

            channel.BasicConsume(queue: queue,
                                    autoAck: true,
                                    consumer: consumer);
            Console.ReadLine();

            //consumer.Received += (sender, e) =>
            //{
            //    var body = e.Body.ToArray();
            //    var replyTo = e.BasicProperties.ReplyTo;
            //    var correlationId = e.BasicProperties.CorrelationId;
            //    var props = channel.CreateBasicProperties();
            //    props.CorrelationId = correlationId;

            //    var message = Encoding.UTF8.GetString(body);
            //    Console.WriteLine($"Message: {message}, ReplyTo: {replyTo}, CorrelationId: {correlationId}");

            //    channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);

            //    var answer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject("ACK"));

            //    channel.BasicPublish(exchange: "",
            //                 routingKey: replyTo,
            //                 basicProperties: props,
            //                 body: answer);
            //};

            //channel.BasicConsume(queue: msgQueue,
            //                     autoAck: false, 
            //                     consumer: consumer);

            //Console.ReadLine();
        }

        //static void Main(string[] args)
        //{
        //    var factory = new ConnectionFactory() { HostName = "localhost" };
        //    using var connection = factory.CreateConnection();
        //    using var channel = connection.CreateModel();
        //    var consumer = new EventingBasicConsumer(channel);
        //    channel.BasicQos(0, 1, true);
        //    var msgQueue = "MessageRPC";

        //    consumer.Received += (sender, e) =>
        //    {
        //        var body = e.Body.ToArray();
        //        var replyTo = e.BasicProperties.ReplyTo;
        //        var correlationId = e.BasicProperties.CorrelationId;
        //        var props = channel.CreateBasicProperties();
        //        props.CorrelationId = correlationId;

        //        var message = Encoding.UTF8.GetString(body);
        //        Console.WriteLine($"Message: {message}, ReplyTo: {replyTo}, CorrelationId: {correlationId}");

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
    }
}
