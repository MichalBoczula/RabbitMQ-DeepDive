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
            ProduceMessageToHeaderExchange();
        }

        private static void ProduceMessageToFanoutExchange()
        {
            FanoutProducer.ProduceMessageToFanoutExchange();
        }

        private static void ProduceMessageToHeaderExchange()
        {
            HeaderProducer.ProduceMessageToHeaderExchange();
        }

        static void ProduceMessagesToTopicExchange()
        {
            TopicProducer.ProduceMessageOnTopicExchange();
        }

        static void ProduceMessagesToDirectExchange()
        {
            DirectProducer.ProduceMessageToDirectExchange();
        }

        static void ProduceMessagesToQueue()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            Producer.ProduceMessagesToQueue(channel);
        }
    }
}
