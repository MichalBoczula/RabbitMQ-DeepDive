using System;

namespace RabbitMQ.Basic
{
    class Program
    {
        static void Main(string[] args)
        {
            Producer.ProduceMessage();
            Console.WriteLine("Message has been sent");

            Consumer.ConsumeMessage();
            Console.WriteLine("Message has been consumed");
        }
    }
}
