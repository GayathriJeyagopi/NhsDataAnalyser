using NHSDataAnalyser.Queries;
using NHSDataAnalyser.Repository;

namespace NHSDataAnalyser.DTO
{
    public class NameQuery : IQuery<IPrescriptionRepository>
    {
        public string BnfName { get; set; }
        public string ShaCodeName { get; set; }
    }
}