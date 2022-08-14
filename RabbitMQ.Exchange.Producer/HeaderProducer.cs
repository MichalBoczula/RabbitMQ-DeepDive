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
    internal class HeaderProducer
    {
        public static void ProduceMessageToHeaderExchange()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var exchange = "headerExchange";

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange, ExchangeType.Headers, true, false, null);

            var message = new { Name = "add, update, get", Message = "Triple actions bro" };
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            var props = channel.CreateBasicProperties();
            props.Headers = new Dictionary<string, object>
            {
                { "add", "add" },
                { "update", "update" },
                { "get", "get" }
            };

            while (true)
            {
                channel.BasicPublish(exchange: exchange,
                             routingKey: string.Empty,
                             basicProperties: props,
                             body: body);

                Thread.Sleep(1000);
            }
        }
    }
}
