using System;
using System.Collections.Generic;
using Moq;
using NHSDataAnalyser.DTO;
using NHSDataAnalyser.Query;
using NHSDataAnalyser.Repository;
using NUnit.Framework;
using ResultState = NHSDataAnalyser.Query.QueryResult<double>.ResultState;

namespace NHSDataAnalyserTest
{
    [TestFixture]
    public class AverageCostOfPrescriptionQueryTests
    {
        private const string SomeBnfName = "SomeName";
        private const string SomeotherBnfName = "SomeOtherBnfName";

        [Test]
        public void Execute_WhenPrescriptionRepositoryNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new AverageCostOfPrescriptionQueryHandler(null));
        }

        [Test]
        public void Execute_WhenPrescriptionRepositoryReturnsAllCostThatMatchesBnfName_ReturnsPassedStateAndAverageCost()
        {
            var mockPrescriptionRespository = MockPrescriptionRespository();
            var handler = new AverageCostOfPrescriptionQueryHandler(mockPrescriptionRespository.Object);
            var query = new NameQuery { BnfName = SomeBnfName };

            var result = handler.Execute(query);

            Assert.That(result.State == ResultState.Pass, "The result state of Query Failed");
            Assert.That(result.Result.Equals(10), "The Average cost is not as expected");
        }

        [Test]
        public void Execute_WhenPrescriptionRepositoryReturnsNone_ReturnsFailedState()
        {
            var handler = new AverageCostOfPrescriptionQueryHandler(new Mock<IPrescriptionRepository>().Object);
            var query = new NameQuery {BnfName = SomeBnfName};
            var result = handler.Execute(query);
            Assert.That(result.State == ResultState.Fail);
        }

        private static Mock<IPrescriptionRepository> MockPrescriptionRespository()
        {
            var mockPrescriptionRespository = new Mock<IPrescriptionRepository>();
            var prescriptionsDetailses = new List<PrescriptionsDetails>
            {
                new PrescriptionsDetails {ActualCost = 15, BnfName = SomeBnfName},
                new PrescriptionsDetails {ActualCost = 10, BnfName = SomeBnfName},
                new PrescriptionsDetails {ActualCost = 5, BnfName = SomeBnfName},
                new PrescriptionsDetails {ActualCost = 5, BnfName = SomeotherBnfName}
            };
            mockPrescriptionRespository.Setup(m => m.GetAll()).Returns(prescriptionsDetailses);
            return mockPrescriptionRespository;
        }
    }
}