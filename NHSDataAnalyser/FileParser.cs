using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSDataAnalyser
{
    internal class FileParser : IFileParser
    {
        private static readonly string _delimiter = ",";
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
            // return ReadFile().Select(line => constructRepository(SplitRow(line)));
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
                row.Split(new[] {_delimiter}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim()).ToList();
        }
    }

    public interface IFileParser
    {
        IEnumerable<List<string>> Parse();
    }
}