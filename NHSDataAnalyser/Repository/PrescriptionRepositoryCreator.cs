using System;
using NHSDataAnalyser.DTO;

namespace NHSDataAnalyser.Repository
{
    internal class PrescriptionRepositoryCreator : IRepositoryCreator<PrescriptionRepository>
    {
        private readonly IFileParser _fileParser;

        public PrescriptionRepositoryCreator(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("fileName is null or empty");
            }
            _fileParser = _fileParser ?? new FileParser(fileName, true);
        }

        /// <summary>
        /// Ctor for Testability.
        /// </summary>
        public PrescriptionRepositoryCreator(string fileName, IFileParser fileParser) : this(fileName)
        {
            _fileParser = fileParser;
        }

        public PrescriptionRepository Create()
        {
            return
                RepositoryFactory.Instance.GetRepositoryInstance<PrescriptionsDetails, PrescriptionRepository>(
                    _fileParser);
        }
    }
}