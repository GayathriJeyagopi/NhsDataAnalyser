using System;
using System.Linq;
using NHSDataAnalyser.DTO;
using NHSDataAnalyser.Repository;
using ResultState = NHSDataAnalyser.Query.QueryResult<int>.ResultState;

namespace NHSDataAnalyser.Query
{
    /// <see cref="IQueryHandler{TQuery,TResult}"/>
    internal class NumberOfPractisesQueryHandler : IQueryHandler<CityNameQuery, QueryResult<int>>
    {
        private readonly IPractiseRepository _practiseRepository;

        public NumberOfPractisesQueryHandler(IPractiseRepository practiseRepository)
        {
            _practiseRepository = practiseRepository;
            if (practiseRepository == null)
            {
                throw new ArgumentNullException("practiseRepository");
            }
        }

        ///<see cref="IQueryHandler{TQuery,TResult}.Execute"/>
        public QueryResult<int> Execute(CityNameQuery query)
        {
            var allPractises = _practiseRepository.GetAll().Where(m => m.Address.City.Equals(query.CityName,StringComparison.InvariantCultureIgnoreCase)).ToList();

            if (!allPractises.Any())
            {
                return FailureMessage(query);
            }

            var count = allPractises.Count;
            Console.WriteLine("Number of GP practises in {0}: {1}", query.CityName, count);
            Console.WriteLine();
            return new QueryResult<int> {State = ResultState.Pass, Result = count};
        }

        private static QueryResult<int> FailureMessage(CityNameQuery query)
        {
            Console.WriteLine("The Query to get the number of practises failed because no matching city name {0} found",
                query.CityName);
            return new QueryResult<int> {State = ResultState.Fail, Result = 0};
        }
    }
}