using System;

namespace NHSDataAnalyser.Repository
{
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
            return (TRepository)Activator.CreateInstance(typeof(TRepository), fileParser);
        }
    }
}