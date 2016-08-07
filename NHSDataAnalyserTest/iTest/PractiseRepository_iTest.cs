using Moq;
using NHSDataAnalyser;
using NHSDataAnalyser.DTO;
using NHSDataAnalyser.Query;
using NHSDataAnalyser.Repository;
using NUnit.Framework;
using ResultState = NHSDataAnalyser.Query.QueryResult<int>.ResultState;

namespace NHSDataAnalyserTest.iTest
{
    /// <summary>
    ///     Component Test for NHSDataAnalyser - These test cases uses all real objects of NHS DataAnalyser component, except
    ///     <see cref="IFileWrapper" /> - which is a mock object. The intention is to mock the external dependency like File system but test the NHsDataAnalyser 
    /// component.
    /// </summary>
    [TestFixture]
    public class PractiseRepository_iTest
    {
        private Mock<IFileWrapper> _mockFileWrapper;
        private IFileParser _fileParser;
        private IPractiseRepository _practiseRepository;
        private const string Somepractisefilename = "somePractiseFileName";
      
        [SetUp]
        public void SetUp()
        {
            _mockFileWrapper = new Mock<IFileWrapper>();
            _mockFileWrapper.Setup(m => m.ReadLines(Somepractisefilename)).Returns(PractiseStubData.Data);
            _fileParser = new FileParser(Somepractisefilename, false, _mockFileWrapper.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _mockFileWrapper = null;
            _fileParser = null;
            _practiseRepository = null;
        }

        [TestCase("STOCKTON", 1 )]
        [TestCase("Stockton", 1)]
        [TestCase("NOTTINGHAM", 2)]
        [TestCase("dERbY", 1)]
        public void NumberOfPractisesQueryTests_FoundMatch_ReturnsPassedResult(string cityName, int expectedNumberOfPractises)
        {
            _practiseRepository = new PractiseRepositoryCreator(Somepractisefilename, _fileParser).Create();

            var result = RunNumberOfPractisesQuery(_practiseRepository, cityName);

            Assert.That(result.Result == expectedNumberOfPractises, "The number of practises in the city is {0}", expectedNumberOfPractises);
            Assert.That(result.State == ResultState.Pass, "Expected Result State is pass");
        }

        [TestCase("", 0)]
        [TestCase(null, 0)]
        [TestCase("XGDGSDFGDSFGDFS", 0)]
        public void NumberOfPractisesQueryTests_NoMatchFound_ReturnsFailedResult(string cityName, int expectedNumberOfPractises)
        {
            _practiseRepository = new PractiseRepositoryCreator(Somepractisefilename, _fileParser).Create();

            var result = RunNumberOfPractisesQuery(_practiseRepository, cityName);

            Assert.That(result.Result == expectedNumberOfPractises, "The number of practises in the city is {0}", expectedNumberOfPractises);
            Assert.That(result.State == ResultState.Fail, "Expected Result State is failed");
        }



        private static QueryResult<int> RunNumberOfPractisesQuery(IPractiseRepository practiseRepository, string cityName)
        {
            var cityNameQuery = new CityNameQuery {CityName = cityName};
            var numberOfPractisesInCity = new NumberOfPractisesQueryHandler(practiseRepository);
            return numberOfPractisesInCity.Execute(cityNameQuery);
        }
    }
}