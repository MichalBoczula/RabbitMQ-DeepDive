using RabbitMQ.Client;
using System;

namespace RabbitMQ.Exchange.Consumer
{
    static class Program
    {
        static void Main(string[] args)
        {
            ConsumeMessageFromTopicExchange();
        }

        private static void ConsumeMessageFromTopicExchange()
        {
            HeaderConsumer.ConsumeMessageFromTopicExchange();
        }

        static void ConsumeMessagesFromTopicExchange()
        {
            TopicConsumer.ConsumeMessageFromTopicExchange();
        }

        static void ConsumeMessageFromDiectQueue()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var queue = "directQueue";

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            DirectConsumer.ConsumeMessageFromDiectQueue(channel, queue);
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
