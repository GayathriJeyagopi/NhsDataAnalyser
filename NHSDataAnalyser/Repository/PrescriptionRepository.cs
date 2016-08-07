using System.Collections.Generic;
using NHSDataAnalyser.DTO;

namespace NHSDataAnalyser.Repository
{
    public class PrescriptionRepository : AbstractRepository<PrescriptionsDetails>, IPrescriptionRepository
    {
        public PrescriptionRepository(IFileParser fileParser) : base(fileParser)
        {
        }

        public override PrescriptionsDetails ConstructRepository(IReadOnlyList<string> column)
        {
            var prescriptionsDetails = new PrescriptionsDetails
            {
                ShaCode = column[0],
                PrimaryCareTrustCodes = column[1],
                PractiseCode = column[2],
                BnfCode = column[3],
                BnfName = column[4],
                NoOfItems = GetNoOfItems(column[5]),
                Nic = GetNic(column[6]),
                ActualCost = GetActualCost(column[7]),
                Period = column[8]
            };
            return prescriptionsDetails;
        }

        private static int? GetNoOfItems(string value)
        {
            int noOfItems;
            return int.TryParse(value, out noOfItems) ? noOfItems : (int?) null;
        }

        private static double? GetNic(string value)
        {
            double nic;
            return double.TryParse(value, out nic) ? nic : (double?) null;
        }

        private static double? GetActualCost(string value)
        {
            double actualCost;
            return double.TryParse(value, out actualCost) ? actualCost : (double?) null;
        }
    }
}