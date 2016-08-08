using System;
using NHSDataAnalyser.DTO;
using NHSDataAnalyser.Query;
using NHSDataAnalyser.Repository;

namespace NHSDataAnalyser
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var inputFileNameCollector = new InputFileNameCollector(new ConsoleWrapper());
            var generalPractise = inputFileNameCollector.Collect("GP Practises");
            if (string.IsNullOrEmpty(generalPractise)) return;

            var prescriptionDetails = inputFileNameCollector.Collect("Prescriptions Details");
            if (string.IsNullOrEmpty(prescriptionDetails)) return;

            var practiseRepository = new PractiseRepositoryCreator(generalPractise).Create();
            var prescriptionRepository = new PrescriptionRepositoryCreator(prescriptionDetails).Create();

            RunUserQueries(practiseRepository, prescriptionRepository);

            Console.ReadKey();
        }

        private static void RunUserQueries(IPractiseRepository practiseRepository,
            IPrescriptionRepository prescriptionRepository)
        {
            ExecuteCityNameQuery(practiseRepository, "London");
            ExecuteAverageCostOfPrescriptionQuery(prescriptionRepository, "Peppermint Oil");

            var combinedRepositoryCreator = new CombinedRepositoryCreator();
            var combinedRepository = combinedRepositoryCreator.Join(prescriptionRepository.GetAll(),
                practiseRepository.GetAll());

            ExecuteTopSpentPostCodesQuery(combinedRepository, 5);
            ExecuteSummaryOfAverageCostForEachRegionQuery(prescriptionRepository, "Flucloxacillin");
            ExecuteNumberOfPractisesInEachRegionQuery(prescriptionRepository, "Q30");
            Console.WriteLine("End of Results....");
        }

        private static void ExecuteNumberOfPractisesInEachRegionQuery(IPrescriptionRepository prescriptionRepository,
            string shaCodeName)
        {
            var shaCodeQuery = new NameQuery {ShaCodeName = shaCodeName};
            var queryHandler = new NumberOfPractisesInShaRegionQueryHandler(prescriptionRepository);
            queryHandler.Execute(shaCodeQuery);
        }

        private static void ExecuteSummaryOfAverageCostForEachRegionQuery(
            IPrescriptionRepository prescriptionRepository, string containsBnfName)
        {
            var summaryOfAverageCostQuery = new ComputeAverageQuery
            {
                ContainsBnfName = containsBnfName
            };
            var summaryOfAverageCostQueryHandler = new SummaryOfAverageCostQueryHandler(prescriptionRepository);
            summaryOfAverageCostQueryHandler.Execute(summaryOfAverageCostQuery);
        }

        private static void ExecuteTopSpentPostCodesQuery(
            CombinedRepositoryCreator.CombinedRepository combinedRepository, int topValue)
        {
            var topSpentPostCodesQuery = new TopSpentPostCodesQuery {TopValue = topValue};
            var topSpentPostCodesQueryHandler = new TopSpentPostCodesByActualCostQueryHandler(combinedRepository);
            topSpentPostCodesQueryHandler.Execute(topSpentPostCodesQuery);
        }

        private static void ExecuteAverageCostOfPrescriptionQuery(IPrescriptionRepository prescriptionRepository,
            string peppermintOil)
        {
            var bnfNameQuery = new NameQuery {BnfName = peppermintOil};
            var averageCostOfPrescriptionQuery =
                new AverageCostOfPrescriptionQueryHandler(prescriptionRepository);
            averageCostOfPrescriptionQuery.Execute(bnfNameQuery);
        }

        private static void ExecuteCityNameQuery(IPractiseRepository practiseRepository, string cityName)
        {
            var cityNameQuery = new CityNameQuery {CityName = cityName};
            var numberOfPractisesInCity = new NumberOfPractisesQueryHandler(practiseRepository);
            numberOfPractisesInCity.Execute(cityNameQuery);
        }
    }
}