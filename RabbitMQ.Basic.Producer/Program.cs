using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ.Basic.Producer
{
    static class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "first-queue",
                                 durable: true, //!!!!!!!!!!!!!!
                                 exclusive: false,//!!!!!!!!!!!! 
                                 autoDelete: false,//!!!!!!!!!!!!!! 
                                 arguments: null);
            
            var props = channel.CreateBasicProperties();
            props.Persistent = true; //!!!!!!!!!!!!!!

            string message = "What's up bro!?";
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            while(true)
            {
                channel.BasicPublish(exchange: "",
                             routingKey: "first-queue",
                             basicProperties: null,
                             body: body);
                
                Thread.Sleep(1000);
            }
        
        }
    }
}
