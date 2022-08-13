using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;

namespace RabbitMQ.Exchange.SecondProducer
{
    class Program
    {
        static void Main(string[] args)
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
                             routingKey: "topic.add",
                             basicProperties: null,
                             body: body);

                Thread.Sleep(1000);
            }
        }
    }
}
