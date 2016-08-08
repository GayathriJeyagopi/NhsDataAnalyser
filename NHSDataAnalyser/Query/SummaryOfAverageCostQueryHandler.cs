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
                return FailureMessage();
            }

            var summaryOfAverageCostForEachRegion =
                ComputeAverageActualCostForEachRegion(query).ToList();

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
            Console.WriteLine("The Summary of actual cost spent query failed");
            return new QueryResult<IEnumerable<SummaryOfAverageCost>> {State = ResultState.Fail};
        }


        private void PrintSummayToConsole(List<SummaryOfAverageCost> summaryOfAverageCostForEachRegion, string bnfName)
        {
            Console.WriteLine("Following are the summary of Average actual cost spent for {0}", bnfName);
            Console.WriteLine("------------------------------------------------------------------------------");

            foreach (var summary in summaryOfAverageCostForEachRegion)
            {
                Console.WriteLine("{0} of SHA Region code {1}:", PrescriptionsDetails.ShaCodeToRegion[summary.ShaCode],
                    summary.ShaCode);
                Console.WriteLine("Average actual cost: {0}", summary.AverageCost);
                Console.WriteLine("Difference to the National Mean {0}", summary.DifferenceToNationalMean);
                Console.WriteLine();
            }
        }

        private IEnumerable<SummaryOfAverageCost> ComputeAverageActualCostForEachRegion(ComputeAverageQuery query)
        {
            var nationalMean = ComputeNationalMean(query);

            if (!nationalMean.HasValue)
            {
                return Enumerable.Empty<SummaryOfAverageCost>();
            }

            Console.WriteLine("National Average cost spent on {0} is {1}", query.ContainsBnfName, nationalMean);
            Console.WriteLine();

            var summaryOfAverageCost = _prescriptionRepository.GetAll()
                .GroupBy(m => m.ShaCode)
                .Where(m => ContainsBnfName(query, m)).Select(GetSummary(nationalMean, query.ContainsBnfName));
            return summaryOfAverageCost;
        }


        private static bool ContainsBnfName(ComputeAverageQuery query, IGrouping<string, PrescriptionsDetails> group)
        {
            return
                group.Any(t => t.BnfName.StartsWith(query.ContainsBnfName, StringComparison.InvariantCultureIgnoreCase));
        }

        private double? ComputeNationalMean(ComputeAverageQuery query)
        {
            var allActualCost =
                _prescriptionRepository.GetAll()
                    .Where(m => m.BnfName.StartsWith(query.ContainsBnfName, StringComparison.InvariantCultureIgnoreCase))
                    .Select(m => m.ActualCost / m.NoOfItems)
                    .ToList();
            return !allActualCost.Any() ? null : allActualCost.Average();
        }

        public Func<IGrouping<string, PrescriptionsDetails>, SummaryOfAverageCost> GetSummary(double? nationalMean,
            string containsBnfName)
        {
            return m =>
                new SummaryOfAverageCost(nationalMean)
                {
                    AverageCost =
                        m.Where(t => t.BnfName.StartsWith(containsBnfName))
                            .Select(t => t.ActualCost / t.NoOfItems)
                            .Average(),
                    ShaCode = m.Key
                };
        }
    }
}