using System.Collections.Generic;

namespace NHSDataAnalyser.DTO
{
    //Data holder class which holds details regarding Prescriptions, ie., BNF - British National Formulary
    internal class PrescriptionsDetails
    {
        internal static readonly Dictionary<string, string> ShaCodeToRegion = new Dictionary<string, string>
        {
            {"Q30", "North East"},
            {"Q31", "North West"},
            {"Q32", "Yorkshire and the Humber"},
            {"Q33", "East Midlands"},
            {"Q34", "West Midlands"},
            {"Q35", "East of England"},
            {"Q36", "London"},
            {"Q37", "South East Coast"},
            {"Q38", "South Central"},
            {"Q39", "South West"}
        };

        internal double? ActualCost;
        internal string BnfCode;
        internal string BnfName;
        internal double? Nic;
        internal int? NoOfItems;
        internal string Period;
        internal string PractiseCode;
        internal string PrimaryCareTrustCodes;
        internal string ShaCode;
    }
}