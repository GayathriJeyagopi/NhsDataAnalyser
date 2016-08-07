using System;
using System.Linq;
using NHSDataAnalyser.DTO;
using NHSDataAnalyser.Repository;
using ResultState = NHSDataAnalyser.Query.QueryResult<double>.ResultState;

namespace NHSDataAnalyser.Query
{
    /// <see cref="IQueryHandler{TQuery,TResult}"/>
    internal class AverageCostOfPrescriptionQueryHandler :
        IQueryHandler<NameQuery, QueryResult<double>>
    {
        private readonly IPrescriptionRepository _prescriptionRepository;

        public AverageCostOfPrescriptionQueryHandler(IPrescriptionRepository prescriptionRepository)
        {
            _prescriptionRepository = prescriptionRepository;
            if (prescriptionRepository == null)
            {
                throw new ArgumentNullException("prescriptionRepository");
            }
        }

        ///<see cref="IQueryHandler{TQuery,TResult}.Execute"/>
        public QueryResult<double> Execute(NameQuery query)
        {
            var allActualCost =
                _prescriptionRepository.GetAll()
                    .Where(m => m.BnfName.Equals(query.BnfName,StringComparison.InvariantCultureIgnoreCase))
                    .Select(m => m.ActualCost)
                    .ToList();
            if (!allActualCost.Any())
            {
                return FailureMessage();
            }

            var averageActualCost = allActualCost.Average();
            if (!averageActualCost.HasValue)
            {
                return FailureMessage();
            }

            Console.WriteLine("Average cost of {0}: {1}", query.BnfName, averageActualCost);
            Console.WriteLine("-------------------------------------------------------------------------------");
            Console.WriteLine();
            return new QueryResult<double> {State = ResultState.Pass, Result = (double) averageActualCost};
        }

        private static QueryResult<double> FailureMessage()
        {
            Console.WriteLine("The Query to get the average actual cost failed");
            return new QueryResult<double> {State = ResultState.Fail};
        }
    }
}