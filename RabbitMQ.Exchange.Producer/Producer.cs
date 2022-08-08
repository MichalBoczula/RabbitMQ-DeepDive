
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ.Exchange.Producer
{
    public static class Producer
    {
        public static void ProduceMessageToDirectExchange(IModel channel, string exchange, string routingKey)
        {
            var props = channel.CreateBasicProperties();
            props.Persistent = true;

            var message = new { Name = "hello", Message = "What's up bro!?" };
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            while (true)
            {
                channel.BasicPublish(exchange: exchange,
                             routingKey: routingKey,
                             basicProperties: null,
                             body: body);

                Thread.Sleep(1000);
            }
        }

        public static void ProduceMessagesToQueue(IModel channel)
        {
            channel.QueueDeclare(queue: "first-queue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var props = channel.CreateBasicProperties();
            props.Persistent = true;

            var message = new { Name = "hello", Message = "What's up bro!?" };
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            while (true)
            {
                channel.BasicPublish(exchange: "",
                             routingKey: "first-queue",
                             basicProperties: null,
                             body: body);

                Thread.Sleep(1000);
            }
        }
    }
}
