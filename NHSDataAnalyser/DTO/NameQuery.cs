using NHSDataAnalyser.Query;

namespace NHSDataAnalyser.DTO
{
    public class NameQuery : IQuery
    {
        public string BnfName { get; set; }
        public string ShaCodeName { get; set; }
    }
}