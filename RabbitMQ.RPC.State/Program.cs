using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.RPC.State
{
    class Program
    {
        static void Main(string[] args)
        {
            var dict = new Dictionary<string, bool>();
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var queue = "StateRPC";
            var queueACK = "StateRpcAck";
            var exchange = "RPCExchange";
            var exchangeAck = "RpcExchangeAck";
            var routingKey = string.Empty;

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            using var channelAck = connection.CreateModel();

            channel.BasicQos(0, 1, true);
            channel.QueueDeclare(queue: queue, true, false, false, null);
            channelAck.BasicQos(0, 1, true);
            channelAck.QueueDeclare(queueACK, true, false, false, null);

            var header = new Dictionary<string, object>
            {
                { "correlation", "correlation"},
            };
            channel.QueueBind(queue, exchange, routingKey, header);

            var headerAck = new Dictionary<string, object>
            {
                { "correlationAck", "correlationAck"},
            };
            channelAck.QueueBind(queueACK, exchangeAck, routingKey, headerAck);

            var props = channel.CreateBasicProperties();
            props.Persistent = true;
            var consumer = new EventingBasicConsumer(channel);
            var consumerAck = new EventingBasicConsumer(channelAck);

            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var id = e.BasicProperties.CorrelationId;
                dict.Add(id, bool.Parse(message));
                System.Console.WriteLine($"added {id}");
            };

            channel.BasicConsume(queue: queue,
                                    autoAck: true,
                                    consumer: consumer);

            consumerAck.Received += (sender, e) =>
            {
                var id = Encoding.UTF8.GetString(e.Body.ToArray());
                if (dict.ContainsKey(id))
                {
                    dict[id] = true;
                    System.Console.WriteLine($"ack {id}");
                }
            };

            channelAck.BasicConsume(queue: queueACK,
                                    autoAck: true,
                                    consumer: consumerAck);

            Console.ReadLine();
        }
    }
}
