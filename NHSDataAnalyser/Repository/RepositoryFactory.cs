using System;

namespace NHSDataAnalyser.Repository
{
    /// <summary>
    ///     Factory class to create Repository, this class makes sure that there is only one Repository created for a
    ///     repository file.
    /// </summary>
    public sealed class RepositoryFactory
    {
        private static RepositoryFactory _instance;

        private RepositoryFactory()
        {
        }

        public static RepositoryFactory Instance
        {
            get { return _instance ?? (_instance = new RepositoryFactory()); }
        }

        public TRepository GetRepositoryInstance<T, TRepository>(IFileParser fileParser) where T : class
            where TRepository : IRepository<T>
        {
            return (TRepository) Activator.CreateInstance(typeof(TRepository), fileParser);
        }
    }
}