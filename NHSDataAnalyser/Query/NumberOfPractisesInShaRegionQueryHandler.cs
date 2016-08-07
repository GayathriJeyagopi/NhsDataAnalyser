using System;
using System.Linq;
using NHSDataAnalyser.DTO;
using NHSDataAnalyser.Repository;
using ResultState = NHSDataAnalyser.Query.QueryResult<int>.ResultState;

namespace NHSDataAnalyser.Query
{
    internal class NumberOfPractisesInShaRegionQueryHandler :
        IQueryHandler<NameQuery, QueryResult<int>>
    {
        private const int FailureCode = -1;
        private readonly IPrescriptionRepository _prescriptionRepository;


        public NumberOfPractisesInShaRegionQueryHandler(IPrescriptionRepository prescriptionRepository)
        {
            if (prescriptionRepository == null)
            {
                throw new ArgumentNullException("prescriptionRepository");
            }
            _prescriptionRepository = prescriptionRepository;
        }

        public QueryResult<int> Execute(NameQuery query)
        {
            if (string.IsNullOrEmpty(query.ShaCodeName))
            {
                throw new ArgumentException("Sha Code Name cannot be null or empty");
            }

            var numberOfPractisesInShaCode = GetPractisesInTheGivenShaCode(query.ShaCodeName);

            if (numberOfPractisesInShaCode == FailureCode)
            {
                return FailureMessage();
            }

            Console.WriteLine("Number of GP practises in {0} and region name {1}  is  {2}: ", query.ShaCodeName,
                PrescriptionsDetails.ShaCodeToRegion[query.ShaCodeName], numberOfPractisesInShaCode);
            Console.WriteLine("-------------------------------------------------------------------------------");
            Console.WriteLine();

            return new QueryResult<int>
            {
                Result = numberOfPractisesInShaCode,
                State = ResultState.Pass
            };
        }

        private QueryResult<int> FailureMessage()
        {
            Console.WriteLine("The Query to get the number of practises in SHA region failed..");
            return new QueryResult<int>
            {
                Result = 0,
                State = ResultState.Fail
            };
        }

        private int GetPractisesInTheGivenShaCode(string shaCode)
        {
            var allDetailsMatchingShaCode = _prescriptionRepository.GetAll()
                .Where(p => p.ShaCode.Equals(shaCode, StringComparison.InvariantCultureIgnoreCase)).ToList();

            if (!allDetailsMatchingShaCode.Any())
            {
                return FailureCode;
            }
           
            return allDetailsMatchingShaCode.Select(pr => pr.PractiseCode).Distinct().Count();
        }
    }
}