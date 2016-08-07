namespace NHSDataAnalyser.DTO
{
    internal class SummaryOfAverageCost
    {
        private readonly double? _nationalMean;
        private double? _averageCost;
        public string ShaCode;
        public double? DifferenceToNationalMean { get; private set; }

        public SummaryOfAverageCost(double? nationalMean)
        {
            _nationalMean = nationalMean;
        }

        public double? AverageCost
        {
            get { return _averageCost; }
            set
            {
                _averageCost = value;
                DifferenceToNationalMean = _nationalMean - AverageCost;
            }
        }
    }
}