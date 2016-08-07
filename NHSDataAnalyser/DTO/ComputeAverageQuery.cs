using NHSDataAnalyser.Queries;
using NHSDataAnalyser.Repository;

namespace NHSDataAnalyser.DTO
{
    internal class ComputeAverageQuery : IQuery<IPrescriptionRepository>
    {
        public string ContainsBnfName { get; set; }
    }
}