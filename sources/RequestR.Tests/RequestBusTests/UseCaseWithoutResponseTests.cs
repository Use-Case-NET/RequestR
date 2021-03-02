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
    public class UseCaseWithoutResponseTests
    {
        #region Request, Use Case, Use Case Factory

        private class TestRequest
        {
        }

        private class TestUseCase : IUseCase<TestRequest>
        {
            public bool WasExecuted { get; private set; }

            public async Task Execute(TestRequest request)
            {
                await Task.Delay(100);

                WasExecuted = true;
            }
        }

        private class UseCaseFactoryMock : UseCaseFactoryBase
        {
            public IUseCase<TestRequest> UseCase { get; set; }

            protected override object CreateInternal(Type type)
            {
                return UseCase;
            }
        }

        #endregion

        private readonly TestUseCase testUseCase;
        private readonly RequestBus requestBus;

        public UseCaseWithoutResponseTests()
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
            requestBus.Send(testRequest);

            Assert.True(testUseCase.WasExecuted);
        }

        [Fact]
        public void CallUseCaseSynchronouslyAndGetValueTypeResponse()
        {
            TestRequest testRequest = new TestRequest();
            int response = requestBus.Send<TestRequest, int>(testRequest);

            Assert.True(testUseCase.WasExecuted);
            Assert.Equal(0, response);
        }

        [Fact]
        public void CallUseCaseSynchronouslyAndGetReferenceTypeResponse()
        {
            TestRequest testRequest = new TestRequest();
            string response = requestBus.Send<TestRequest, string>(testRequest);

            Assert.True(testUseCase.WasExecuted);
            Assert.Null(response);
        }

        [Fact]
        public void CallUseCaseAsynchronouslyWithoutResponse()
        {
            TestRequest testRequest = new TestRequest();
            requestBus.SendAsync(testRequest).Wait();

            Assert.True(testUseCase.WasExecuted);
        }

        [Fact]
        public void CallUseCaseAsynchronouslyAndGetValueTypeResponse()
        {
            TestRequest testRequest = new TestRequest();
            int response = requestBus.SendAsync<TestRequest, int>(testRequest).Result;

            Assert.True(testUseCase.WasExecuted);
            Assert.Equal(0, response);
        }

        [Fact]
        public void CallUseCaseAsynchronouslyAndGetReferenceTypeResponse()
        {
            TestRequest testRequest = new TestRequest();
            string response = requestBus.SendAsync<TestRequest, string>(testRequest).Result;

            Assert.True(testUseCase.WasExecuted);
            Assert.Null(response);
        }
    }
}