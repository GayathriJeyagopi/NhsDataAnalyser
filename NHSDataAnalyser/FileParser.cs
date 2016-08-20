using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSDataAnalyser
{
    /// <summary>
    /// Parses the given CSV file, Currently only supports comma separated files.
    /// </summary>
    internal class FileParser : IFileParser
    {
        private const string Delimiter = ",";
        private readonly string _file;
        private readonly IFileWrapper _fileWrapper;
        private readonly bool _hasHeader;

        public FileParser(string file, bool hasHeader)
            : this(file, hasHeader, new FileWrapper())
        {
            if (string.IsNullOrEmpty(file))
            {
                throw new ArgumentException("file is null or empty");
            }
        }

        /// <summary>
        ///     Ctor for dependency injection, for Testability.
        /// </summary>
        public FileParser(string file, bool hasHeader, IFileWrapper fileWrapper)
        {
            _file = file;
            _fileWrapper = fileWrapper;
            _hasHeader = hasHeader;
        }

        public IEnumerable<List<string>> Parse()
        {
            return ReadFile().Select(SplitRow);
        }

        private IEnumerable<string> ReadFile()
        {
            if (_hasHeader)
            {
                return _fileWrapper.ReadLines(_file).Skip(1).ToList();
            }
            return _fileWrapper.ReadLines(_file).ToList();
        }

        private List<string> SplitRow(string row)
        {
            return
                row.Split(new[] {Delimiter}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim()).ToList();
        }
    }
}