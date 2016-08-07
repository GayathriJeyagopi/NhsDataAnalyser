using System;
using System.Linq;
using Moq;
using NHSDataAnalyser.DTO;
using NHSDataAnalyser.Query;
using NHSDataAnalyser.Repository;
using NUnit.Framework;
using ResultState =
    NHSDataAnalyser.Query.QueryResult<System.Collections.Generic.IEnumerable<NHSDataAnalyser.DTO.SummaryOfTotalCost>>.
        ResultState;

namespace NHSDataAnalyserTest
{
    [TestFixture]
    public class TopSpentPostCodesByActualCostQueryTests
    {
        [TestCase(0)]
        [TestCase(-1)]
        public void Execute_TopValueIsInvalid_ThrowsArgumentException(int topValue)
        {
            var query = new TopSpentPostCodesQuery {TopValue = topValue};
            var queryHandler = new TopSpentPostCodesByActualCostQueryHandler(new Mock<ICombinedRepository>().Object);

            Assert.Throws<ArgumentException>(() => queryHandler.Execute(query));
        }


        [Test]
        public void Execute_CombinedRepositoryHasPractisesGivenaTopValue_ReturnsSummaryOfTopSpentAndPassedState()
        {
            var query = new TopSpentPostCodesQuery {TopValue = 2};

            var mockCombinedRepository = CreateMockCombinedRepository(new[]
                {
                    CreateCombinedDetails(CreatPractise("practiseCode1", "Ox1 2jj"), CreatePrescriptionDetails(40, "practiseCode1")),
                    CreateCombinedDetails(CreatPractise("practiseCode2", "OX4 4QL"),CreatePrescriptionDetails(20, "practiseCode2")),
                    CreateCombinedDetails(CreatPractise("practiseCode3", "OX4 4QL"),CreatePrescriptionDetails(60, "practiseCode2"))
                });

            var queryHandler = new TopSpentPostCodesByActualCostQueryHandler(mockCombinedRepository.Object);

            var result = queryHandler.Execute(query);

            Assert.That(result.State == ResultState.Pass, "Result State is Pass");
            var summaryOfTotalCosts = result.Result.ToList();
            Assert.That(summaryOfTotalCosts.Count == 2, "The top spent value summary count is 2");

            Assert.That(summaryOfTotalCosts[0].PostCode.Equals("OX4 4QL"));
            Assert.That(summaryOfTotalCosts[0].TotalCost == 60);
            Assert.That(summaryOfTotalCosts[1].PostCode.Equals("Ox1 2jj"));
            Assert.That(summaryOfTotalCosts[1].TotalCost == 40);
        }

        [Test]
        public void Execute_CombinedRepositoryIsEmptyGivenaTopValue_ReturnsFailedState()
        {
            var query = new TopSpentPostCodesQuery {TopValue = 1};
            var queryHandler = new TopSpentPostCodesByActualCostQueryHandler(new Mock<ICombinedRepository>().Object);

            var result = queryHandler.Execute(query);

            Assert.That(result.State == ResultState.Fail, "The Result state should be failed as the Repository is empty");
            Assert.That(!result.Result.Any(), "The Combined Repository is Empty and hence the result has to be empty.");
        }

        [Test]
        public void Execute_CombinedRepositoryIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new TopSpentPostCodesByActualCostQueryHandler(null));
        }

        private Mock<ICombinedRepository> CreateMockCombinedRepository(CombinedDetails[] combinedDetailses)
        {
            var mockCombinedRepository = new Mock<ICombinedRepository>();
            mockCombinedRepository.Setup(m => m.GetAll()).Returns(combinedDetailses);
            return mockCombinedRepository;
        }

        private static CombinedDetails CreateCombinedDetails(Practise practise, PrescriptionsDetails prescriptionDetails)
        {
            return new CombinedDetails
            {
                Practises = practise,
                PrescriptionsDetailses = prescriptionDetails
            };
        }

        private PrescriptionsDetails CreatePrescriptionDetails(double actualCost, string practiseCode)
        {
            return new PrescriptionsDetails { ActualCost = actualCost, PractiseCode = practiseCode };
        }

        private Practise CreatPractise(string practisecode, string postcode)
        {
            return new Practise { Address = new Address { PostCode = postcode }, PractiseCode = practisecode };
        }
    }
}