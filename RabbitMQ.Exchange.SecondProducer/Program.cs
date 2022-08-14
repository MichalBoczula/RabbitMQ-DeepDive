using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RabbitMQ.Exchange.SecondProducer
{
    class Program
    {
        static void Main(string[] args)
        {
            StartSecondProducerHeader();
        }

        private static void StartSecondProducerHeader()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var exchange = "headerExchange";
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange, ExchangeType.Headers, true, false, null);

            var message = new { Name = "get", Message = "only get brooo" };
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            var props = channel.CreateBasicProperties();
            props.Headers = new Dictionary<string, object>()
            {
                {"get", "get"},
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

        private static void StartSecondTopicConsumer()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var exchange = "topicExchange";
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange, ExchangeType.Topic, true, false, null);

            var message = new { Name = "hello", Message = "What's up bro!?" };
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            while (true)
            {
                channel.BasicPublish(exchange: exchange,
                             routingKey: "topic.add",
                             basicProperties: null,
                             body: body);

                Thread.Sleep(1000);
            }
        }
    }
}
