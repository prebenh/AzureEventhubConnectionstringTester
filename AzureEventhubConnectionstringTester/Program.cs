using Microsoft.Azure.EventHubs;
using System;
using System.Linq;
using System.Threading.Tasks;
using static System.Console;

namespace AzureEventhubConnectionstringTester
{
    internal class Program
    {
        private static void Main()
        {
            var program = new Program();
            program.MainAsync().GetAwaiter().GetResult();
        }

        private const string ConnectionString =
            "<your eventhub connection string>";

        private const string EntityPath = "<your entitypath>";

        private async Task MainAsync()
        {
            var builder = new EventHubsConnectionStringBuilder(ConnectionString)
            {
                EntityPath = EntityPath
            };

            var client = EventHubClient.CreateFromConnectionString(builder.ToString());

            WriteLine("Getting partition info");
            var info = await client.GetRuntimeInformationAsync();

            foreach (var partitionId in info.PartitionIds)
            {
                WriteLine($"Creating receiver for partition [{partitionId}]");
                var receiver = client.CreateReceiver(PartitionReceiver.DefaultConsumerGroupName,
                   partitionId, EventPosition.FromStart(), new ReceiverOptions());

                var messages = await receiver.ReceiveAsync(10, TimeSpan.FromMilliseconds(10));
                WriteLine($"Got [{messages?.Count() ?? 0}] messages from partion [{partitionId}]");
            }

            Write("Finished");
            ReadLine();
        }
    }
}