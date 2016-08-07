using System.Collections.Generic;
using System.IO;

namespace NHSDataAnalyser
{
    /// <summary>
    /// Wrapper class around System.IO.File API's
    /// </summary>
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