using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;

namespace RabbitMQ.Exchange.Producer
{
    static class Program
    {
        static void Main(string[] args)
        {
            ProduceMessagesToDirectExchange();
        }

        static void ProduceMessagesToQueue()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            Producer.ProduceMessagesToQueue(channel);
        }

        static void ProduceMessagesToDirectExchange()
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

            Producer.ProduceMessageToDirectExchange(channel, exchange, routingKey);
        }
    }
}
