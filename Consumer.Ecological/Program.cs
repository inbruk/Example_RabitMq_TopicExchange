using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer.Ecological
{
    class Program
    {
        const String exchangeName = "topic_logs";
        const String routingKey = "*.*.electric";

        static void Main(String[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchangeName, ExchangeType.Topic);
                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queueName, exchangeName, routingKey);


                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, e) =>
                {
                    var body = e.Body;
                    var message = Encoding.UTF8.GetString(body.ToArray());
                    Console.WriteLine("Received message: {0}", message);
                };

                channel.BasicConsume(queueName, true, consumer);
                Console.WriteLine($"It subscribe our consumer to queue '{queueName}'");
                Console.ReadLine();
            }
        }
    }
}
