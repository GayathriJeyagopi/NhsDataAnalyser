using System.Collections.Generic;
using System.IO;

namespace NHSDataAnalyser
{
    public class FileWrapper : IFileWrapper
    {
        public bool Exists(string fileName)
        {
            return File.Exists(fileName);
        }

        public IEnumerable<string> ReadLines(string fileName)
        {
            return File.ReadLines(fileName); 
        }
    }
}