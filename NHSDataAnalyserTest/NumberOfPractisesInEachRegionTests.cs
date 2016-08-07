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
    public class NumberOfPractisesInEachRegionTest
    {
        [Test]
        public void Execute_WhenPrescriptionRepositoryNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new NumberOfPractisesInShaRegionQueryHandler(null));
        }

        [Test]
        public void Execute_WhenShaCodeQuerIsEmpty_ThrowsArgumentException()
        {
            var query = new NameQuery ();
            var handler = new NumberOfPractisesInShaRegionQueryHandler(new Mock<IPrescriptionRepository>().Object);
            Assert.Throws<ArgumentException>(() => handler.Execute(query));
        }

        [Test]
        public void Execute_WhenPrescriptionRepositoryReturnsNone_ReturnsFailedState()
        {
            var query = new NameQuery{ShaCodeName = "SomeCode"};
            var handler = new NumberOfPractisesInShaRegionQueryHandler(new Mock<IPrescriptionRepository>().Object);
            var result = handler.Execute(query);

            Assert.That(result.State == QueryResult<int>.ResultState.Fail,
                "The Prescription Repository is empty and hence the result state has to be failed");
        }

        [Test]
        public void Execute_WhenPrescriptionRepositoryReturnsSomePractises_ReturnsPractisesCountAndPassedState()
        {
            var query = new NameQuery {ShaCodeName = "Q31"};
            var mockPrescriptionRepository = new Mock<IPrescriptionRepository>();
            mockPrescriptionRepository.Setup(m => m.GetAll())
                .Returns(new List<PrescriptionsDetails>
                {
                    new PrescriptionsDetails {ShaCode = "Q31", PractiseCode = "AQ1"},
                    new PrescriptionsDetails {ShaCode = "Q31", PractiseCode = "AQ1"},
                    new PrescriptionsDetails {ShaCode = "Q31", PractiseCode = "AQ1"},
                    new PrescriptionsDetails {ShaCode = "Q31", PractiseCode = "AQ2"},
                    new PrescriptionsDetails {ShaCode = "Q36", PractiseCode = "AAAA"},
                    new PrescriptionsDetails {ShaCode = "Q36", PractiseCode = "AAAA"},
                    new PrescriptionsDetails {ShaCode = "Q36", PractiseCode = "AAAA"}
                });

            var handler = new NumberOfPractisesInShaRegionQueryHandler(mockPrescriptionRepository.Object);

            var result = handler.Execute(query);

            Assert.That(result.State == QueryResult<int>.ResultState.Pass);
            Assert.That(result.Result.Equals(2));
        }
    }
}