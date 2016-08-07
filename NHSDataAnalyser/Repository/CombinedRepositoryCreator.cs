using System.Collections.Generic;
using System.Linq;
using NHSDataAnalyser.DTO;

namespace NHSDataAnalyser.Repository
{
    internal class CombinedRepositoryCreator
    {
        public CombinedRepository Join(IEnumerable<PrescriptionsDetails> prescriptionRepositories,
            IEnumerable<Practise> practises)
        {
            var joinedRepository = prescriptionRepositories.Join(practises,
                prescriptionsDetails => prescriptionsDetails.PractiseCode,
                practise => practise.PractiseCode,
                (prescriptionsDetails, practise) =>
                    new CombinedDetails {Practises = practise, PrescriptionsDetailses = prescriptionsDetails});
            return new CombinedRepository(joinedRepository);
        }

        internal class CombinedRepository : ICombinedRepository
        {
            private readonly IEnumerable<CombinedDetails> _repository;

            public CombinedRepository(IEnumerable<CombinedDetails> repository)
            {
                _repository = repository;
            }

            public IEnumerable<CombinedDetails> GetAll()
            {
                return _repository.ToList();
            }
        }
    }
}