using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NHSDataAnalyser;
using NHSDataAnalyser.DTO;
using NHSDataAnalyser.Query;
using NHSDataAnalyser.Repository;
using NUnit.Framework;
using ResultStateDouble = NHSDataAnalyser.Query.QueryResult<double>.ResultState;

namespace NHSDataAnalyserTest.iTest
{
    /// <summary>
    ///     Component Test for NHSDataAnalyser - These test cases uses all real objects of NHS DataAnalyser component, except
    ///     <see cref="IFileWrapper" /> - which is a mock object. The intention is to mock the external dependency like File
    ///     system but test the NHsDataAnalyser
    ///     component.
    /// </summary>
    [TestFixture]
    public class PrescriptionRepository_iTest
    {
        [SetUp]
        public void SetUp()
        {
            _mockFileWrapper = new Mock<IFileWrapper>();
            _mockFileWrapper.Setup(m => m.ReadLines(Someprescriptionfilename)).Returns(PrescriptionStubData.Data);
            _fileParser = new FileParser(Someprescriptionfilename, true, _mockFileWrapper.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _mockFileWrapper = null;
            _fileParser = null;
            _prescriptionRepository = null;
        }

        private Mock<IFileWrapper> _mockFileWrapper;
        private IFileParser _fileParser;
        private IPrescriptionRepository _prescriptionRepository;
        private const string Someprescriptionfilename = "somePrescriptionFileName";

        [TestCase("Peppermint Oil", 69.8825)]
        [TestCase("PEPPERMINT OIL", 69.8825)]
        [TestCase("Glyceryl Trinitrate", 57.7875)]
        [TestCase("Aluminium Hydroxide", 4.24)]
        public void AverageCostOfPrescriptionQueryTests_FoundMatch_ReturnsPassedResult(string bnfName,
            double expectedAverageCost)
        {
            _prescriptionRepository = new PrescriptionRepositoryCreator(Someprescriptionfilename, _fileParser).Create();

            var result = RunAverageCostOfPrescriptionQuery(_prescriptionRepository, bnfName);

            Assert.That(result.Result == expectedAverageCost, "The Average cost of prescription is {0}",
                expectedAverageCost);
            Assert.That(result.State == ResultStateDouble.Pass, "Expected Result State is pass");
        }

        [TestCase("", 0)]
        [TestCase(null, 0)]
        [TestCase("XGDGSDFGDSFGDFS", 0)]
        public void AverageCostOfPrescriptionQueryTests_NoMatchFound_ReturnsFailedResult(string bnfName,
            double expectedAverageCost)
        {
            _prescriptionRepository = new PrescriptionRepositoryCreator(Someprescriptionfilename, _fileParser).Create();

            var result = RunAverageCostOfPrescriptionQuery(_prescriptionRepository, bnfName);

            Assert.That(result.Result == expectedAverageCost, "The Average cost of prescription is {0}",
                expectedAverageCost);
            Assert.That(result.State == ResultStateDouble.Fail, "Expected Result State is failed");
        }

        [TestCase("Q30", 2)]
        [TestCase("Q31", 3)]
        [TestCase("Q32", 1)]
        public void NumberOfPractisesInEachRegionQueryTests_FoundMatch_ReturnsCountPassedResult(string shaCodeName,
            int numberOfPractises)
        {
            _prescriptionRepository = new PrescriptionRepositoryCreator(Someprescriptionfilename, _fileParser).Create();

            var result = ExecuteNumberOfPractisesInEachRegionQuery(_prescriptionRepository, shaCodeName);

            Assert.That(result.Result == numberOfPractises, "The Number Of Practises in {0} is {1}", shaCodeName,
                numberOfPractises);

            Assert.That(result.State == QueryResult<int>.ResultState.Pass, "Expected Result State is pass");
        }
       
        [Test]
        public void NumberOfPractisesInEachRegionQueryTests_NoMatch_ReturnsFailedState()
        {
            _prescriptionRepository = new PrescriptionRepositoryCreator(Someprescriptionfilename, _fileParser).Create();

            var result = ExecuteNumberOfPractisesInEachRegionQuery(_prescriptionRepository, "XGDGSDFGDSFGDFS");

            Assert.That(result.Result == 0, "The number of practises in Sha region {0} is {1}", "XGDGSDFGDSFGDFS",
                 0);
            Assert.That(result.State == QueryResult<int>.ResultState.Fail, "Expected Result State is failed");
        }


        [TestCase("Glycerol", new[] {"Q31"}, new[] {10.0}, new[] {0.0},
            TestName = "Summary of Average cost for Glycerol")]
        [TestCase("Peppermint Oil", new[] {"Q30", "Q31", "Q32"}, new[] {53.18, 35.52, 137.66},
            new[] {16.71, 34.36, -67.78}, TestName = "Summary of Average cost For Peppermint Oil")]
        [TestCase("Glyceryl Trinitrate", new[] {"Q31", "Q32"}, new[] {66.35, 32.1}, new[] {-8.56, 25.69},
            TestName = "Summary of Average cost for Glyceryl Trinitrate")]
        public void SummaryOfAverageCostOfPrescriptionForEachRegionQueryTests_FoundMatch_ReturnsPassedResult(
            string bnfName, string[] regionCode, double[] averageCost, double[] differenceToNationalMean)
        {
            _prescriptionRepository = new PrescriptionRepositoryCreator(Someprescriptionfilename, _fileParser).Create();

            var resultSummary = RunSummaryOfAverageCostForEachRegion(_prescriptionRepository, bnfName);

            var summaryOfAverageCosts = resultSummary.Result.ToList();
            for (var i = 0; i < summaryOfAverageCosts.Count; i++)
            {
                Assert.That(summaryOfAverageCosts[i].AverageCost.HasValue && Math.Round(summaryOfAverageCosts[i].AverageCost.Value, 2) == averageCost[i],
                    "The Average cost of prescription is {0}", summaryOfAverageCosts[i].AverageCost.Value);
                Assert.That(
                    Math.Round(summaryOfAverageCosts[i].DifferenceToNationalMean.Value, 2) ==
                    differenceToNationalMean[i], "The Average cost of prescription is {0}", differenceToNationalMean[i]);
                Assert.That(summaryOfAverageCosts[i].ShaCode.Equals(summaryOfAverageCosts[i].ShaCode),
                    "The Region Code is {0}", summaryOfAverageCosts[i].ShaCode);
            }

            Assert.That(resultSummary.State == QueryResult<IEnumerable<SummaryOfAverageCost>>.ResultState.Pass,
                "Expected Result State is pass");
        }


        [TestCase("")]
        [TestCase(null)]
        [TestCase("XGDGSDFGDSFGDFS")]
        public void SummaryOfAverageCostOfPrescriptionForEachRegionQueryTests_NoMatchFound_ReturnsFailedResult(string bnfName)
        {
            _prescriptionRepository = new PrescriptionRepositoryCreator(Someprescriptionfilename, _fileParser).Create();

            var resultSummary = RunSummaryOfAverageCostForEachRegion(_prescriptionRepository, bnfName);

            Assert.That(resultSummary.State == QueryResult<IEnumerable<SummaryOfAverageCost>>.ResultState.Fail, "Expected Result State is failed");
        }

        private QueryResult<IEnumerable<SummaryOfAverageCost>> RunSummaryOfAverageCostForEachRegion(
            IPrescriptionRepository prescriptionRepository, string bnfName)
        {
            var summaryOfAverageCostQuery = new ComputeAverageQuery {ContainsBnfName = bnfName};
            var summaryOfAverageCostQueryHandler = new SummaryOfAverageCostQueryHandler(prescriptionRepository);
            return summaryOfAverageCostQueryHandler.Execute(summaryOfAverageCostQuery);
        }


        private static QueryResult<double> RunAverageCostOfPrescriptionQuery(
            IPrescriptionRepository prescriptionRepository, string bnfName)
        {
            var bnfNameQuery = new NameQuery {BnfName = bnfName};
            var averageCostOfPrescriptionQueryHandler = new AverageCostOfPrescriptionQueryHandler(prescriptionRepository);
            return averageCostOfPrescriptionQueryHandler.Execute(bnfNameQuery);
        }

        private static QueryResult<int> ExecuteNumberOfPractisesInEachRegionQuery(IPrescriptionRepository prescriptionRepository,
           string shaCodeName)
        {
            var shaCodeQuery = new NameQuery { ShaCodeName = shaCodeName };
            var queryHandler = new NumberOfPractisesInShaRegionQueryHandler(prescriptionRepository);
            return queryHandler.Execute(shaCodeQuery);
        }
    }
}