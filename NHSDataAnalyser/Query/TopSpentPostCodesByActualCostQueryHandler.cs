using System;
using System.Collections.Generic;
using System.Linq;
using NHSDataAnalyser.DTO;
using NHSDataAnalyser.Repository;
using ResultState =
    NHSDataAnalyser.Query.QueryResult<System.Collections.Generic.IEnumerable<NHSDataAnalyser.DTO.SummaryOfTotalCost>>.
        ResultState;

namespace NHSDataAnalyser.Query
{
    /// <see cref="IQueryHandler{TQuery,TResult}"/>
    internal class TopSpentPostCodesByActualCostQueryHandler :
        IQueryHandler<TopSpentPostCodesQuery, QueryResult<IEnumerable<SummaryOfTotalCost>>>
    {
        private readonly ICombinedRepository _combinedRepository;


        public TopSpentPostCodesByActualCostQueryHandler(ICombinedRepository combinedRepository)
        {
            if (combinedRepository == null)
            {
                throw new ArgumentNullException("combinedRepository");
            }
            _combinedRepository = combinedRepository;
        }

        ///<see cref="IQueryHandler{TQuery,TResult}.Execute"/>
        public QueryResult<IEnumerable<SummaryOfTotalCost>> Execute(TopSpentPostCodesQuery query)
        {
            if (query.TopValue <= 0)
            {
                throw new ArgumentException("Top value cannot be zero or negative number");
            }

            var summaryOfTopSpent = GetPractisesWhichHasTopSpentActualCost(query.TopValue).ToList();

            if (!summaryOfTopSpent.Any())
            {
                return FailureMessage();
            }

            PrintSummaryToConsole(query, summaryOfTopSpent);

            return new QueryResult<IEnumerable<SummaryOfTotalCost>>
            {
                Result = summaryOfTopSpent,
                State = ResultState.Pass
            };
        }

        private static QueryResult<IEnumerable<SummaryOfTotalCost>> FailureMessage()
        {
            Console.WriteLine("The Query to get Top spent value failed..");
            return new QueryResult<IEnumerable<SummaryOfTotalCost>>
            {
                State = ResultState.Fail,
                Result = new List<SummaryOfTotalCost>()
            };
        }

        private static void PrintSummaryToConsole(TopSpentPostCodesQuery query,
            List<SummaryOfTotalCost> summaryOfTopSpent)
        {
            Console.WriteLine("Summary of Top spent {0} PostCodes are: ", query.TopValue);
            foreach (var summary in summaryOfTopSpent)
            {
                Console.WriteLine("PostCode {0} has totally spent :{1} ", summary.PostCode, summary.TotalCost);
            }

            Console.WriteLine("-------------------------------------------------------------------------------");
            Console.WriteLine();
        }

        private IEnumerable<SummaryOfTotalCost> GetPractisesWhichHasTopSpentActualCost(int topValue)
        {
            return _combinedRepository.GetAll().GroupBy(p => p.Practises.PractiseCode)
                .Select(CreateSummaryOfTotalCost)
                .OrderByDescending(p => p.TotalCost).Take(topValue);
        }

        private static SummaryOfTotalCost CreateSummaryOfTotalCost(IGrouping<string, CombinedDetails> group)
        {
            return new SummaryOfTotalCost
            {
                TotalCost = group.Sum(t => t.PrescriptionsDetailses.ActualCost),
                PostCode = SelectPostCode(group)
            };
        }

        private static string SelectPostCode(IGrouping<string, CombinedDetails> group)
        {
            return group.Where(t => t.Practises.PractiseCode.Equals(group.Key))
                .Select(t => t.Practises.Address.PostCode)
                .First();
        }
    }
}