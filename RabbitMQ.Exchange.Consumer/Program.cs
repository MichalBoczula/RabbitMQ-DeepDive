using RabbitMQ.Client;
using System;

namespace RabbitMQ.Exchange.Consumer
{
    static class Program
    {
        static void Main(string[] args)
        {
            ConsumeMessageFromHeaderExchange();
        }

        private static void ConsumeMessageFromFanoutExchange()
        {
            FanoutConsumer.ConsumeMessageFromFanoutExchange();
        }

        private static void ConsumeMessageFromHeaderExchange()
        {
            HeaderConsumer.ConsumeMessageFromHeaderExchange();
        }

        static void ConsumeMessagesFromTopicExchange()
        {
            TopicConsumer.ConsumeMessageFromTopicExchange();
        }

        static void ConsumeMessageFromDiectQueue()
        {
            DirectConsumer.ConsumeMessageFromDiectQueue();
        }

        static void ConsumeMesasgeFromQueue()
        {
            Consumer.ConsumeMessageFromQueue();
        }
    }
}
