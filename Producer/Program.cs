using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RabbitMQ.Client;

namespace Producer
{
    class Program
    {
        const String exchangeName = "topic_logs";

        private static readonly List<String> brands = new List<String>() { "BMW", "Audi", "Tesla", "Hyundai" };
        private static readonly List<String> colors = new List<String>() { "red", "green", "blue", "white" };
        private static readonly List<String> fuels = new List<String>() { "gasoline", "gas", "electric", "diesel" };
        private static readonly Random rnd = new Random();

        static String GenerateCarRoutingKey()
        {
            String brand = brands[rnd.Next(0,4)];
            String color = colors[rnd.Next(0,4)];
            String fuel = fuels[rnd.Next(0,4)];

            if (brand == brands[2])
                fuel = fuels[2];

            String routingKey = $"{brand}.{color}.{fuel}";
            return routingKey;
        }


        static Func<Task> CreateTask()
        {
            return () =>
            {
                var counter = 0;
                do
                {
                    var timeToSleep = rnd.Next(1000, 3000);
                    Thread.Sleep(timeToSleep);

                    String routingKey = GenerateCarRoutingKey();

                    var factory = new ConnectionFactory() { HostName = "localhost" };
                    using (var connection = factory.CreateConnection())
                    using (var channel = connection.CreateModel())
                    {
                        channel.ExchangeDeclare(exchangeName, ExchangeType.Topic);

                        var message = $"'{routingKey}' message from producer N:{counter++}";
                        var body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish(exchange: exchangeName, routingKey: routingKey, basicProperties: null, body: body);

                        Console.WriteLine(message);
                    }
                }
                while (true);
            };
        }

        static void Main(String[] args)
        {
            Task.Run(CreateTask());
            Console.ReadKey();
        }
    }
}