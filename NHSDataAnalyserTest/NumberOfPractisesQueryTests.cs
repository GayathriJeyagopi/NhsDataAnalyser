using System;
using System.Collections.Generic;
using Moq;
using NHSDataAnalyser.DTO;
using NHSDataAnalyser.Query;
using NHSDataAnalyser.Repository;
using NUnit.Framework;

namespace NHSDataAnalyserTest
{
    [TestFixture]
    public class NumberOfPractisesQueryTests
    {
        [Test]
        public void Execute_WhenPractiseRepositoryNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new NumberOfPractisesQueryHandler(null));
        }

        [Test]
        public void Execute_WhenPractiseRepositoryReturnsNone_ReturnsFailedState()
        {
            var query = new CityNameQuery();
            var handler = new NumberOfPractisesQueryHandler(new Mock<IPractiseRepository>().Object);
            var result = handler.Execute(query);

            Assert.That(result.State == QueryResult<int>.ResultState.Fail,
                "The Practise Repository is empty and hence the result state has to be failed");
        }

        [Test]
        public void Execute_WhenPractiseRepositoryReturnsSomePractises_ReturnsCityCountAndPassedState()
        {
            var query = new CityNameQuery {CityName = "Oxford"};
            var mockPractiseRepository = new Mock<IPractiseRepository>();
            mockPractiseRepository.Setup(m => m.GetAll())
                .Returns(new List<Practise>
                {
                    new Practise {Address = new Address {City = "Oxford"}},
                    new Practise {Address = new Address {City = "OXFORD"}}
                });
            var handler = new NumberOfPractisesQueryHandler(mockPractiseRepository.Object);

            var result = handler.Execute(query);

            Assert.That(result.State == QueryResult<int>.ResultState.Pass);
            Assert.That(result.Result.Equals(2));
        }
    }
}