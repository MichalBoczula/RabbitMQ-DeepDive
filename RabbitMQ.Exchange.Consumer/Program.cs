using RabbitMQ.Client;
using System;

namespace RabbitMQ.Exchange.Consumer
{
    static class Program
    {
        static void Main(string[] args)
        {
            ProduceMessagesToDirectExchange();
        }

        static void ProduceMessagesToDirectExchange()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var queue = "directQueue";

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            Consumer.ConsumeMessageFromDiectQueue(channel, queue);
        }

        static void ConsumeMesasgeFromQueue()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            Consumer.ConsumeMessageFromQueue(channel);
        }
    }
}
