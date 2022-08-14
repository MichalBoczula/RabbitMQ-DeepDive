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
    internal static class FanoutProducer
    {
        public static void ProduceMessageToFanoutExchange()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var exchange = "fanoutExchange";

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange, ExchangeType.Fanout, true, false, null);

            var message = new { Name = "Fun", Message = "Let's get some fun brooo." };
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            var props = channel.CreateBasicProperties();
            props.Headers = new Dictionary<string, object>
            {
                { "zoba", "nie dziala" }
            };

            while (true)
            {
                channel.BasicPublish(exchange: exchange,
                             routingKey: "tez nie dziala",
                             basicProperties: props,
                             body: body);

                Thread.Sleep(1000);
            }
        }
    }
}
