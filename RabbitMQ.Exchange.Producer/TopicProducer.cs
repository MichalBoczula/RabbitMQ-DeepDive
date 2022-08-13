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
    internal static class TopicProducer
    {
        public static void ProduceMessageOnTopicExchange()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var exchange = "topicExchange";

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange, ExchangeType.Topic, true, false, null);

            var message = new { Name = "hello", Message = "What's up bro!?" };
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            while (true)
            {
                channel.BasicPublish(exchange: exchange,
                             routingKey: "topic.update",
                             basicProperties: null,
                             body: body);

                Thread.Sleep(1000);
            }
        }
    }
}
