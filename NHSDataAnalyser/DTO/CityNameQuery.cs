using NHSDataAnalyser.Queries;
using NHSDataAnalyser.Repository;

namespace NHSDataAnalyser.DTO
{
    public class CityNameQuery : IQuery<IPractiseRepository>
    {
        public string CityName { get; set; }
    }
}