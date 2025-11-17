using System.Diagnostics;
using Xunit;

namespace DustInTheWind.RequestR.Tests.RequestBusTests;

public class CancelUseCaseWithoutResponseTests
{
    #region Request, Use Case, Use Case Factory

    private class TestRequest
    {
    }

    private class TestUseCase : IUseCase<TestRequest>
    {
        public TimeSpan FinishedTime { get; private set; }

        public async Task Execute(TestRequest request, CancellationToken cancellationToken)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                await Task.Delay(1000, cancellationToken);
            }
            finally
            {
                stopwatch.Stop();

                FinishedTime = stopwatch.Elapsed;
            }
        }
    }

    private class UseCaseFactoryMock : UseCaseFactoryBase
    {
        public TestUseCase UseCase { get; set; }

        protected override object CreateInternal(Type type)
        {
            return UseCase;
        }
    }

    #endregion

    private readonly TestUseCase testUseCase;
    private readonly RequestBus requestBus;

    public CancelUseCaseWithoutResponseTests()
    {
        testUseCase = new TestUseCase();
        UseCaseFactoryMock useCaseFactory = new UseCaseFactoryMock
        {
            UseCase = testUseCase
        };
        requestBus = new RequestBus(useCaseFactory);

        requestBus.RegisterUseCase<TestUseCase>();
    }

    [Fact]
    public async void CancelAsynchronousUseCaseWithoutResponse()
    {
        TestRequest testRequest = new TestRequest();
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(100);
        CancellationToken cancellationToken = cancellationTokenSource.Token;

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await requestBus.SendAsync(testRequest, cancellationToken);
        });

        Assert.InRange(testUseCase.FinishedTime, TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(150));
    }

    [Fact]
    public async void CancelAsynchronousUseCaseWithValueTypeResponse()
    {
        TestRequest testRequest = new TestRequest();
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(100);
        CancellationToken cancellationToken = cancellationTokenSource.Token;

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            int response = await requestBus.SendAsync<TestRequest, int>(testRequest, cancellationToken);
        });

        Assert.InRange(testUseCase.FinishedTime, TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(150));
    }

    [Fact]
    public async void CancelAsynchronousUseCaseWithReferenceTypeResponse()
    {
        TestRequest testRequest = new TestRequest();
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(100);
        CancellationToken cancellationToken = cancellationTokenSource.Token;

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            string response = await requestBus.SendAsync<TestRequest, string>(testRequest, cancellationToken);
        });

        Assert.InRange(testUseCase.FinishedTime, TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(150));
    }
}