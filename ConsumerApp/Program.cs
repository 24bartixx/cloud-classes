// See https://aka.ms/new-console-template for more information

using ConsumerApp.Consumers;
using Shared;

var brokerClient = new RabbitBrokerClient("amqps://ibhwhnoo:sA0oIjbD8BlAH1YypXoa-qzih091Ufsl@sparrow.rmq.cloudamqp.com/ibhwhnoo");
brokerClient.InitializeAsync().Wait();

// 2x Type 1
for (int i = 0; i < 2; i++)
{
    var consumer = new SimpleConsumer<Type1Event>(i + 1, brokerClient); 
    consumer.SubscribeAsync().Wait();
}

// 1x Type 2
for (int i = 0; i < 1; i++)
{
    var consumer = new SimpleConsumer<Type2Event>(i + 3, brokerClient); 
    consumer.SubscribeAsync().Wait();
}

Console.WriteLine("All consumers run. Press any key to exit...");
Console.ReadKey();