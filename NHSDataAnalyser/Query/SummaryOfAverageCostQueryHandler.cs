using System;
using System.Collections.Generic;
using System.Linq;
using NHSDataAnalyser.DTO;
using NHSDataAnalyser.Repository;
using ResultState =
    NHSDataAnalyser.Query.QueryResult<System.Collections.Generic.IEnumerable<NHSDataAnalyser.DTO.SummaryOfAverageCost>>.
        ResultState;

namespace NHSDataAnalyser.Query
{
    /// <see cref="IQueryHandler{TQuery,TResult}" />
    internal class SummaryOfAverageCostQueryHandler :
        IQueryHandler<ComputeAverageQuery, QueryResult<IEnumerable<SummaryOfAverageCost>>>
    {
        private readonly IPrescriptionRepository _prescriptionRepository;

        /// <see cref="IQueryHandler{TQuery,TResult}.Execute" />
        public SummaryOfAverageCostQueryHandler(IPrescriptionRepository prescriptionRepository)
        {
            if (prescriptionRepository == null)
            {
                throw new ArgumentNullException("prescriptionRepository");
            }
            _prescriptionRepository = prescriptionRepository;
        }

        public QueryResult<IEnumerable<SummaryOfAverageCost>> Execute(ComputeAverageQuery query)
        {
            if (string.IsNullOrEmpty(query.ContainsBnfName))
            {
                throw new ArgumentException(query.ContainsBnfName);
            }

            IEnumerable<PrescriptionsDetails> prescriptionsDetailses =
                _prescriptionRepository.GetAll()
                    .Where(m => m.BnfName.StartsWith(query.ContainsBnfName, StringComparison.InvariantCultureIgnoreCase));

            List<SummaryOfAverageCost> summaryOfAverageCostForEachRegion = ComputeAverageActualCostForEachRegion(prescriptionsDetailses, query.ContainsBnfName).ToList();

            if (!summaryOfAverageCostForEachRegion.Any())
            {
                return FailureMessage();
            }

            PrintSummayToConsole(summaryOfAverageCostForEachRegion, query.ContainsBnfName);

            return new QueryResult<IEnumerable<SummaryOfAverageCost>>
            {
                Result = summaryOfAverageCostForEachRegion,
                State = ResultState.Pass
            };
        }

        private static QueryResult<IEnumerable<SummaryOfAverageCost>> FailureMessage()
        {
            Console.WriteLine("The Summary of actual cost spent per prescription query failed");
            return new QueryResult<IEnumerable<SummaryOfAverageCost>> {State = ResultState.Fail};
        }


        private void PrintSummayToConsole(IEnumerable<SummaryOfAverageCost> summaryOfAverageCostForEachRegion,
            string bnfName)
        {
            Console.WriteLine("Following are the summary of Average actual cost spent Per prescription for {0}", bnfName);
            Console.WriteLine("------------------------------------------------------------------------------");

            foreach (SummaryOfAverageCost summary in summaryOfAverageCostForEachRegion)
            {
                Console.WriteLine("{0} of SHA Region code {1}:", PrescriptionsDetails.ShaCodeToRegion[summary.ShaCode],
                    summary.ShaCode);
                Console.WriteLine("Average actual cost per Prescription: {0}", summary.AverageCost);
                Console.WriteLine("Difference to the National Mean {0}", summary.DifferenceToNationalMean);
                Console.WriteLine();
            }
        }

        private IEnumerable<SummaryOfAverageCost> ComputeAverageActualCostForEachRegion(IEnumerable<PrescriptionsDetails> prescriptionsDetailses, string containsBnfName)
        {
            double? nationalMean = ComputeNationalMean(prescriptionsDetailses);

            if (!nationalMean.HasValue)
            {
                return Enumerable.Empty<SummaryOfAverageCost>();
            }

            Console.WriteLine("National Average cost spent on {0} is {1}", containsBnfName, nationalMean);
            Console.WriteLine();

            return prescriptionsDetailses.GroupBy(m => m.ShaCode).Select(GetSummary(nationalMean, containsBnfName));
        }


        private double? ComputeNationalMean(IEnumerable<PrescriptionsDetails> prescriptionsDetailses)
        {
           return prescriptionsDetailses.Average(m => m.ActualCost / m.NoOfItems);
        }

        public Func<IGrouping<string, PrescriptionsDetails>, SummaryOfAverageCost> GetSummary(double? nationalMean,string containsBnfName)
        {
            return m =>
                new SummaryOfAverageCost(nationalMean)
                {
                    AverageCost = m.Average(t => t.ActualCost / t.NoOfItems),
                    ShaCode = m.Key
                };
        }
    }
}