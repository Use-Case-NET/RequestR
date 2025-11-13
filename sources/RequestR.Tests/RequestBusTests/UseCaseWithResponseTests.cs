// RequestR
// Copyright (C) 2021-2025 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using Xunit;

namespace DustInTheWind.RequestR.Tests.RequestBusTests;

public class UseCaseWithResponseTests
{
    #region Request, Use Case, Use Case Factory

    private class TestRequest
    {
    }

    private class TestUseCase : IUseCase<TestRequest, string>
    {
        public bool WasExecuted { get; private set; }

        public async Task<string> Execute(TestRequest request, CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);

            WasExecuted = true;

            return "response";
        }
    }

    private class UseCaseFactoryMock : UseCaseFactoryBase
    {
        public IUseCase<TestRequest, string> UseCase { get; set; }

        protected override object CreateInternal(Type type)
        {
            return UseCase;
        }
    }

    #endregion

    private readonly TestUseCase testUseCase;
    private readonly RequestBus requestBus;

    public UseCaseWithResponseTests()
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
    public void CallUseCaseSynchronouslyWithoutResponse()
    {
        TestRequest testRequest = new TestRequest();
        requestBus.Process(testRequest);

        Assert.True(testUseCase.WasExecuted);
    }

    [Fact]
    public void CallUseCaseSynchronouslyAndGetIncorrectTypeResponse()
    {
        TestRequest testRequest = new TestRequest();

        Assert.Throws<ResponseCastException>(() =>
        {
            requestBus.Process<TestRequest, int>(testRequest);
        });
    }

    [Fact]
    public void CallUseCaseSynchronouslyAndGetCorrectTypeResponse()
    {
        TestRequest testRequest = new TestRequest();
        string response = requestBus.Process<TestRequest, string>(testRequest);

        Assert.True(testUseCase.WasExecuted);
        Assert.Equal("response", response);
    }

    [Fact]
    public void CallUseCaseAsynchronouslyWithoutResponse()
    {
        TestRequest testRequest = new TestRequest();
        requestBus.ProcessAsync(testRequest).Wait();

        Assert.True(testUseCase.WasExecuted);
    }

    [Fact]
    public void CallUseCaseAsynchronouslyAndGetIncorrectTypeResponse()
    {
        TestRequest testRequest = new TestRequest();

        Assert.ThrowsAsync<ResponseCastException>(async () =>
        {
            await requestBus.ProcessAsync<TestRequest, int>(testRequest);
        });
    }

    [Fact]
    public void CallUseCaseAsynchronouslyAndGetCorrectTypeResponse()
    {
        TestRequest testRequest = new TestRequest();
        string response = requestBus.ProcessAsync<TestRequest, string>(testRequest).Result;

        Assert.True(testUseCase.WasExecuted);
        Assert.Equal("response", response);
    }
}