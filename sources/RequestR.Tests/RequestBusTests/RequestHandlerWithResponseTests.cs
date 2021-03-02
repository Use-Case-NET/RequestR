// RequestR
// Copyright (C) 2021 Dust in the Wind
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

using System;
using System.Threading.Tasks;
using Xunit;

namespace DustInTheWind.RequestR.Tests.RequestBusTests
{
    public class RequestHandlerWithResponseTests
    {
        #region Request, Use Case, Use Case Factory

        private class TestRequest
        {
        }

        private class TestRequestHandler : IRequestHandlerAsync<TestRequest, string>
        {
            public bool WasExecuted { get; private set; }

            public async Task<string> Handle(TestRequest request)
            {
                await Task.Delay(100);

                WasExecuted = true;

                return "response";
            }
        }

        private class RequestHandlerFactoryMock : IRequestHandlerFactory
        {
            public TestRequestHandler RequestHandler { get; set; }

            public T Create<T>()
            {
                return default;
            }

            public object Create(Type type)
            {
                return RequestHandler;
            }
        }

        #endregion

        private readonly TestRequestHandler testRequestHandler;
        private readonly RequestBus requestBus;

        public RequestHandlerWithResponseTests()
        {
            testRequestHandler = new TestRequestHandler();
            RequestHandlerFactoryMock requestHandlerFactory = new RequestHandlerFactoryMock
            {
                RequestHandler = testRequestHandler
            };
            requestBus = new RequestBus(requestHandlerFactory);

            requestBus.RegisterHandler<TestRequestHandler>();
        }

        [Fact]
        public void CallRequestHandlerSynchronouslyWithoutResponse()
        {
            TestRequest testRequest = new TestRequest();
            requestBus.Send(testRequest);

            Assert.True(testRequestHandler.WasExecuted);
        }

        [Fact]
        public void CallRequestHandlerSynchronouslyAndGetIncorrectTypeResponse()
        {
            TestRequest testRequest = new TestRequest();
            int response = requestBus.Send<TestRequest, int>(testRequest);

            Assert.True(testRequestHandler.WasExecuted);
            Assert.Equal(0, response);
        }

        [Fact]
        public void CallUseCaseSynchronouslyAndGetCorrectTypeResponse()
        {
            TestRequest testRequest = new TestRequest();
            string response = requestBus.Send<TestRequest, string>(testRequest);

            Assert.True(testRequestHandler.WasExecuted);
            Assert.Equal("response", response);
        }

        [Fact]
        public void CallUseCaseAsynchronouslyWithoutResponse()
        {
            TestRequest testRequest = new TestRequest();
            requestBus.SendAsync(testRequest).Wait();

            Assert.True(testRequestHandler.WasExecuted);
        }

        [Fact]
        public void CallUseCaseAsynchronouslyAndGetIncorrectTypeResponse()
        {
            TestRequest testRequest = new TestRequest();
            int response = requestBus.SendAsync<TestRequest, int>(testRequest).Result;

            Assert.True(testRequestHandler.WasExecuted);
            Assert.Equal(0, response);
        }

        [Fact]
        public void CallUseCaseAsynchronouslyAndGetCorrectTypeResponse()
        {
            TestRequest testRequest = new TestRequest();
            string response = requestBus.SendAsync<TestRequest, string>(testRequest).Result;

            Assert.True(testRequestHandler.WasExecuted);
            Assert.Equal("response", response);
        }
    }
}