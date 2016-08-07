using System;

namespace NHSDataAnalyser
{
    public class InputFileNameCollector
    {
        private readonly IConsoleWrapper _consoleWrapper;
        private readonly IFileWrapper _fileWrapper;

        public InputFileNameCollector(IConsoleWrapper consoleWrapper) : this(consoleWrapper, new FileWrapper())
        {
        }

        /// <summary>
        ///     Ctor for dependency injection for testability.
        /// </summary>
        public InputFileNameCollector(IConsoleWrapper consoleWrapper, IFileWrapper fileWrapper)
        {
            _consoleWrapper = consoleWrapper;
            _fileWrapper = fileWrapper;
        }

        public string Collect(string dataFor)
        {
            _consoleWrapper.WriteLine(string.Format("Enter the {0} File Name with Full path or D&D the file from explorer :",
                dataFor));
            var fileName = _consoleWrapper.ReadLine().Replace("\"", string.Empty);
            _consoleWrapper.WriteLine("----------------------------------------------------------------");
            return CollectValidFileName(fileName) ? fileName : string.Empty;
        }

        private bool CollectValidFileName(string fileName)
        {
            if (_fileWrapper.Exists(fileName))
            {
                return true;
            }
            _consoleWrapper.WriteLine(
                "The File name is invalid, please Press Enter and try again or press any key to exit");
            var keyInfo = _consoleWrapper.ReadKey();
            if (keyInfo.Key != ConsoleKey.Enter)
            {
                return false;
            }
            fileName = _consoleWrapper.ReadLine();
            CollectValidFileName(fileName);
            return true;
        }
    }
}