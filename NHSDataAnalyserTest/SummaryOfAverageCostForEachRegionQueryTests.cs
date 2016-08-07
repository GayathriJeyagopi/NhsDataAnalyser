using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NHSDataAnalyser.DTO;
using NHSDataAnalyser.Query;
using NHSDataAnalyser.Repository;
using NUnit.Framework;
using ResultState =
    NHSDataAnalyser.Query.QueryResult<System.Collections.Generic.IEnumerable<NHSDataAnalyser.DTO.SummaryOfAverageCost>>.
        ResultState;

namespace NHSDataAnalyserTest
{
    [TestFixture]
    public class SummaryOfAverageCostForEachRegionQueryTests
    {
        private const string SomeotherBnfName = "SomeOtherBnfName";
        private readonly string SomeBnfName = "SomeName";

        [Test]
        public void Execute_WhenPrescriptionRepositoryNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new SummaryOfAverageCostQueryHandler(null));
        }

        [Test]
        public void Execute_WhenPrescriptionRepositoryHasMatchingBnfName_ReturnsResultStatePassedAndReturnsSummary()
        {
            var query = new ComputeAverageQuery {ContainsBnfName = SomeBnfName};
            var mockPrescriptionRepository = MockPrescriptionRepository();
            var handler = new SummaryOfAverageCostQueryHandler(mockPrescriptionRepository.Object);

            var result = handler.Execute(query);

            Assert.That(result.State == ResultState.Pass);
            Assert.That(result.Result.Count(p => p.ShaCode == "Q31"), Is.EqualTo(1));
            Assert.That(result.Result.Count(p => p.ShaCode == "Q36"), Is.EqualTo(1));
            var averageCostForQ31 = result.Result.Where(m => m.ShaCode.Equals("Q31")).Select(a =>a.AverageCost).First();
            Assert.That(averageCostForQ31.HasValue && Math.Round(averageCostForQ31.Value, 2) == 40.33);

            var averageCostForQ36 = result.Result.Where(m => m.ShaCode.Equals("Q36")).Select(a =>a.AverageCost).First();
            Assert.That(averageCostForQ36.HasValue && Math.Round(averageCostForQ36.Value, 2) == 15.50);
        }


        [Test]
        public void Execute_WhenPrescriptionRepositoryReturnsEmptyThatMatchesBnfName_ReturnsResultStateFailed()
        {
            var query = new ComputeAverageQuery {ContainsBnfName = SomeBnfName};
            var mockPrescriptionRepository = new Mock<IPrescriptionRepository>();
            mockPrescriptionRepository.Setup(m => m.GetAll())
                .Returns(new List<PrescriptionsDetails> {new PrescriptionsDetails {BnfName = "SomeOtherName"}});
            var handler = new SummaryOfAverageCostQueryHandler(mockPrescriptionRepository.Object);

            var result = handler.Execute(query);

            Assert.That(result.State == ResultState.Fail);
        }

        [Test]
        public void Execute_WhenPrescriptionRepositoryReturnsNone_ReturnsResultStateFailed()
        {
            var query = new ComputeAverageQuery {ContainsBnfName = SomeBnfName};
            var mockPrescriptionRepository = new Mock<IPrescriptionRepository>();
            var handler = new SummaryOfAverageCostQueryHandler(mockPrescriptionRepository.Object);

            var result = handler.Execute(query);

            Assert.That(result.State == ResultState.Fail);
        }

        private Mock<IPrescriptionRepository> MockPrescriptionRepository()
        {
            var mockPrescriptionRepository = new Mock<IPrescriptionRepository>();
            mockPrescriptionRepository.Setup(m => m.GetAll())
                .Returns(new List<PrescriptionsDetails>
                {
                    new PrescriptionsDetails {BnfName = SomeBnfName, ActualCost = 100, ShaCode = "Q31"},
                    new PrescriptionsDetails {BnfName = SomeBnfName, ActualCost = 10, ShaCode = "Q31"},
                    new PrescriptionsDetails {BnfName = SomeBnfName, ActualCost = 20, ShaCode = "Q36"},
                    new PrescriptionsDetails {BnfName = SomeBnfName, ActualCost = 10.99, ShaCode = "Q36"},
                    new PrescriptionsDetails {BnfName = SomeBnfName, ActualCost = 10.99, ShaCode = "Q31"},
                    new PrescriptionsDetails {BnfName = SomeotherBnfName, ActualCost = 0.99, ShaCode = "Q31"}
                });

            return mockPrescriptionRepository;
        }
    }
}