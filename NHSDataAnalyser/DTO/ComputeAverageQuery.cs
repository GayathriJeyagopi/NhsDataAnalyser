using NHSDataAnalyser.Query;

namespace NHSDataAnalyser.DTO
{
    internal class ComputeAverageQuery : IQuery
    {
        public string ContainsBnfName { get; set; }
    }
}