using System.Collections.Generic;

namespace NHSDataAnalyser
{
    public interface IFileWrapper
    {
        bool Exists(string fileName);

        IEnumerable<string> ReadLines(string fileName);
    }
}