using System;
using System.Diagnostics;
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

            var sw = new Stopwatch();
            sw.Start();

            var practiseRepository = new PractiseRepositoryCreator(generalPractise).Create();
            var prescriptionRepository = new PrescriptionRepositoryCreator(prescriptionDetails).Create();
            
            RunUserQueries(practiseRepository, prescriptionRepository);
            Console.WriteLine("Total time taken for the analysis: {0}",sw.ElapsedMilliseconds);
            sw.Stop();

            Console.ReadKey();
        }


        private static void RunUserQueries(IPractiseRepository practiseRepository,
            IPrescriptionRepository prescriptionRepository)
        {
            var sw = new Stopwatch();
            sw.Start();
            ExecuteCityNameQuery(practiseRepository, "London");
            Console.WriteLine("Time taken to execute City Name Query: {0}",sw.ElapsedMilliseconds);

            sw.Restart();
            ExecuteAverageCostOfPrescriptionQuery(prescriptionRepository, "Peppermint Oil");
            Console.WriteLine("Time taken to execute Average Cost of Prescription: {0}", sw.ElapsedMilliseconds);

            sw.Restart();
            var combinedRepositoryCreator = new CombinedRepositoryCreator();
            var combinedRepository = combinedRepositoryCreator.Join(prescriptionRepository.GetAll(),
                practiseRepository.GetAll());

            ExecuteTopSpentPostCodesQuery(combinedRepository, 5);
            Console.WriteLine("Time taken to execute Top Spent Post Codes Query: {0}", sw.ElapsedMilliseconds);

            sw.Restart();
            ExecuteSummaryOfAverageCostForEachRegionQuery(prescriptionRepository, "Flucloxacillin");
            Console.WriteLine("Time taken to execute Summary Of Average Cost For Each Region: {0}", sw.ElapsedMilliseconds);

            sw.Restart();
            ExecuteNumberOfPractisesInEachRegionQuery(prescriptionRepository, "Q30");
            Console.WriteLine("Time taken to execute Number Of Practises In Each Region: {0}", sw.ElapsedMilliseconds);
            sw.Stop();
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