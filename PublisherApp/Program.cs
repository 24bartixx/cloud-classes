using PublisherApp.Publishers;
using Shared;

var brokerClient = new RabbitBrokerClient("amqps://ibhwhnoo:sA0oIjbD8BlAH1YypXoa-qzih091Ufsl@sparrow.rmq.cloudamqp.com/ibhwhnoo");
brokerClient.InitializeAsync().Wait();

// 3x Type 1
for (int i = 0; i < 3; i++)
{
    var publisher1 = new ConstantTimePublisher(i + 1, 1000 + 1000 * i, brokerClient); 
    publisher1.RunAsync().Wait();
}

// 1x Type 2
var publisher2 = new RandomTimePublisher<Type2Event>(4, brokerClient);
_ = publisher2.RunAsync(CancellationToken.None);

// 1x Type 3
var publisher3 = new RandomTimePublisher<Type3Event>(5, brokerClient);
_ = publisher3.RunAsync(CancellationToken.None);

Console.WriteLine("All publishers run. Press any key to exit...");
Console.ReadKey();