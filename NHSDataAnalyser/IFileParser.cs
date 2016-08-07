using System.Collections.Generic;

namespace NHSDataAnalyser
{
    public interface IFileParser
    {
        IEnumerable<List<string>> Parse();
    }
}