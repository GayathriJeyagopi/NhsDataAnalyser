using System.Collections.Generic;

namespace NHSDataAnalyserTest.iTest
{
    public static class PrescriptionStubData
    {
        public static IEnumerable<string> Data = new List<string>
        {
            " SHA,PCT,PRACTICE,BNF CODE,BNF NAME                              ,ITEMS  ,NIC        ,ACT COST   ,PERIOD",
            "Q30,5.00E+01,A81001,0101010C0,Aluminium Hydroxide                     ,1,4.57,4.24,201109",
            "Q30,5.00E+01,A81001,0102000T0,Peppermint Oil                          ,3,26.15,24.19,201109",
            "Q30,5.00E+01,A81001,0104020N0,Opium & Morphine                        ,2,5.56,5.19,201109",
            "Q30,5.00E+01,A81040,0102000T0,Peppermint Oil                          ,10,88.8,82.16,201109",
            "Q31,5F7,P88023,0102000T0,Peppermint Oil                          ,5,38.37,35.52,201109",
            "Q31,5F7,P88023,0107040A0,Glyceryl Trinitrate                     ,1,34.8,32.1,201109",
            "Q31,5F7,P88023,0206010F0,Glyceryl Trinitrate                     ,32,91.4,85.27,201109",
            "Q31,5F7,P88024,0206010F0,Glyceryl Trinitrate                     ,30,87.59,81.68,201109",
            "Q32,5EF,B81005,0102000T0,Peppermint Oil                          ,14,149.03,137.66,201109",
            "Q32,5EF,B81005,0107040A0,Glyceryl Trinitrate                     ,1,34.8,32.1,201109",
            "Q31,5EF,B81005,0107040A0,Glycerol                     ,1,34.8,10,201109"
        };
    }
}