using System.Collections.Generic;
using System.Linq;
using Moq;
using NHSDataAnalyser;
using NHSDataAnalyser.DTO;
using NHSDataAnalyser.Query;
using NHSDataAnalyser.Repository;
using NUnit.Framework;

namespace NHSDataAnalyserTest.iTest
{
    /// <summary>
    ///     Component Test for NHSDataAnalyser - These test cases uses all real objects of NHS DataAnalyser component, except
    ///     <see cref="IFileWrapper" /> - which is a mock object. The intention is to mock the external dependency like File
    ///     system but test the NHsDataAnalyser component.
    /// </summary>
    [TestFixture]
    public class CombinedRepository_iTest
    {
        [SetUp]
        public void SetUp()
        {
            _mockFileWrapper = new Mock<IFileWrapper>();
            _mockFileWrapper.Setup(m => m.ReadLines(Somepractisefilename)).Returns(PractiseStubData.Data);
            _mockFileWrapper.Setup(m => m.ReadLines(Someprescriptionfilename)).Returns(PrescriptionStubData.Data);

            var fileParser = new FileParser(Somepractisefilename, false, _mockFileWrapper.Object);

            var practiseRepository = new PractiseRepositoryCreator(Somepractisefilename, fileParser).Create();

            fileParser = new FileParser(Someprescriptionfilename, true, _mockFileWrapper.Object);
            var prescriptionRepository =
                new PrescriptionRepositoryCreator(Someprescriptionfilename, fileParser).Create();

            var combinedRepositoryCreator = new CombinedRepositoryCreator();
            _combinedRepository = combinedRepositoryCreator.Join(prescriptionRepository.GetAll(),
                practiseRepository.GetAll());
        }

        [TearDown]
        public void TearDown()
        {
            _mockFileWrapper = null;
            _combinedRepository = null;
        }

        private Mock<IFileWrapper> _mockFileWrapper;

        private const string Somepractisefilename = "somePractiseFileName";
        private CombinedRepositoryCreator.CombinedRepository _combinedRepository;
        private const string Someprescriptionfilename = "somePrescriptionFileName";

        [TestCase(5, new[] {"DN18 5ER", "SK8 3JD", "TS23 2DG", "SK8 4DG", "TS18 1HU"},
            new[] {179.76, 152.89, 82.16, 81.68, 33.62}, TestName = "Top 5 PostCodes")]
        [TestCase(2, new[] { "DN18 5ER", "SK8 3JD" }, new[] { 179.76, 152.89}, TestName = "Top 2 PostCodes")]
        [TestCase(1, new[] {"DN18 5ER"},new[] { 179.76}, TestName = "Top 1 PostCode")]
        public void CombinedRepository_ValidInput_ReturnsTopSpentPostcode_AndPassedState(int topValue, string[] postCodes,
            double[] totalCost)
        {
            var result = RunTopSpentPostCodesByActualCostQuery(topValue).Result.ToList();

            for (var i = 0; i < result.Count; i++)
            {
                Assert.That(result[i].PostCode.Equals(postCodes[i]), "The Expected PostCode is {0}", postCodes[i]);
                Assert.That(result[i].TotalCost == totalCost[i], "The Expected Total cost is {0}", totalCost[i]);
            }
        }

        private QueryResult<IEnumerable<SummaryOfTotalCost>> RunTopSpentPostCodesByActualCostQuery(int topValue)
        {
            var topSpentPostCodesQuery = new TopSpentPostCodesQuery {TopValue = topValue};
            var topSpentPostCodesQueryHandler = new TopSpentPostCodesByActualCostQueryHandler(_combinedRepository);
            return topSpentPostCodesQueryHandler.Execute(topSpentPostCodesQuery);
        }
    }
}