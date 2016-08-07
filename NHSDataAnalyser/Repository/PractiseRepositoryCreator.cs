using System;
using NHSDataAnalyser.DTO;

namespace NHSDataAnalyser.Repository
{
    internal class PractiseRepositoryCreator : IRepositoryCreator<IPractiseRepository>
    {
        private readonly IFileParser _fileParser;

        public PractiseRepositoryCreator(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("fileName is null or empty");
            }
           _fileParser =  _fileParser ?? new FileParser(fileName, false);
        }
        
        /// <summary>
        /// Constructor for Testability
        /// </summary>
        public PractiseRepositoryCreator(string fileName, IFileParser fileParser) : this(fileName)
        {
            _fileParser = fileParser;
        }

        public IPractiseRepository Create()
        {
            return RepositoryFactory.Instance.GetRepositoryInstance<Practise, PractiseRepository>(_fileParser);
        }
    }
}