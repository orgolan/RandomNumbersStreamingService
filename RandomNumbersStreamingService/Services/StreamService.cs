using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace RandomNumbersService.Services
{
    public class RandomNumbersStreamingService : RandomNumbersStreaming.RandomNumbersStreamingBase
    {
        private readonly ILogger<RandomNumbersStreamingService> _logger;
        public RandomNumbersStreamingService(ILogger<RandomNumbersStreamingService> logger)
        {
            _logger = logger;
        }

        public override async Task StartStreaming(Empty request, IServerStreamWriter<StreamReply> responseStream, ServerCallContext context)
        {
            var random = new Random();
            while (!context.CancellationToken.IsCancellationRequested)
            {
                var randomNumber = random.Next(1, 1001);
                await responseStream.WriteAsync(new StreamReply { RandomNumber = randomNumber.ToString() });
                await Task.Delay(250);
            }

            _logger.LogInformation("Streaming completed");
        }
    }
}