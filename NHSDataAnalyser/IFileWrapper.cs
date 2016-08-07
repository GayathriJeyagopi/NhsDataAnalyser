using System.Collections.Generic;

namespace NHSDataAnalyser
{
    /// <summary>
    /// Interface for File Wrapper APIs 
    /// </summary>
    public interface IFileWrapper
    {
        /// <summary>
        /// Returns true if the file exists, false otherwise.
        /// </summary>
        bool Exists(string fileName);

        /// <summary>
        /// Read all the content of the file.
        /// </summary>
        IEnumerable<string> ReadLines(string fileName);
    }
}