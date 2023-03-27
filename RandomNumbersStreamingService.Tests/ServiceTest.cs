using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Core.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using RandomNumbersService.Services;

namespace RandomNumbersService.Tests
{

    public class RandomNumbersStreamingServiceTests
    {
        [Fact]
        public async Task TestRandomNumberInRange()
        {
            // Arrange
            var mockStreamWriter = new Mock<IServerStreamWriter<StreamReply>>();
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(5));
            var context = TestServerCallContext.Create(
                                             method: nameof(RandomNumbersStreaming.RandomNumbersStreamingBase)
                                            , host: "localhost"
                                            , deadline: DateTime.Now.AddMinutes(30)
                                            , requestHeaders: new Metadata()
                                            , cancellationToken: cts.Token
                                            , peer: "localhost"
                                            , authContext: null
                                            , contextPropagationToken: null
                                            , writeHeadersFunc: (metadata) => Task.CompletedTask
                                            , writeOptionsGetter: () => new WriteOptions()
                                            , writeOptionsSetter: (writeOptions) => { }
                                            );

            var mockContext = new Mock<ServerCallContext>(MockBehavior.Strict, new object[] { new CallOptions(), "method", new DateTime(), new Metadata(), cts.Token });
            
            // Act
            var service = new RandomNumbersStreamingService(Mock.Of<ILogger<RandomNumbersStreamingService>>());
            await service.StartStreaming(new Empty(), mockStreamWriter.Object, context);

            // Assert
            mockStreamWriter.Verify(s => s.WriteAsync(It.Is<StreamReply>(r => int.Parse(r.RandomNumber) >= 1 && int.Parse(r.RandomNumber) <= 1000)), Times.AtLeastOnce());
        }
    }
}
