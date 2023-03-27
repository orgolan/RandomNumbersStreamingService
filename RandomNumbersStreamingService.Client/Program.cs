using Grpc.Core;
using Grpc.Net.Client;
using RandomNumbersService;

const string ADDRESS = "https://localhost:7071";
using var channel = GrpcChannel.ForAddress(ADDRESS);
var client = new RandomNumbersStreaming.RandomNumbersStreamingClient(channel);

using var call = client.StartStreaming(new Google.Protobuf.WellKnownTypes.Empty(), new CallOptions());

HashSet<int> receivedNumbers = new HashSet<int>();

// Start a new thread to read messages from the server
var readTask = Task.Run(async () =>
{
    while (await call.ResponseStream.MoveNext())
    {
        var message = call.ResponseStream.Current;
        Console.WriteLine($"Server: {message.RandomNumber}");

        var isAdded = receivedNumbers.Add(int.Parse(message.RandomNumber));
        if (!isAdded) // already exists
        {
            call.Dispose();
            break;
        }
    }
});

// Wait for the read thread to finish
await readTask;

Console.WriteLine("Press any key to exit...");
Console.ReadKey();