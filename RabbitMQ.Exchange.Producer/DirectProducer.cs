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
    public static class DirectProducer
    {
        public static void ProduceMessageToDirectExchange()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var exchange = "directExchange";
            var queue = "directQueue";
            var routingKey = "direct";

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange, ExchangeType.Direct, true, false, null);
            channel.QueueDeclare(queue: queue, true, false, false, null);
            channel.QueueBind(queue, exchange, routingKey);

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
    }
}
