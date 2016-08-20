using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSDataAnalyser.Repository
{
    /// <summary>
    ///     Abstract Repository class - Constructs Repository from the datasource. how to construct the repository is left to
    ///     the overridden classes.
    /// </summary>
    public abstract class AbstractRepository<T> : IRepository<T> where T : class
    {
        protected AbstractRepository(IFileParser fileParser)
        {
            if (fileParser == null)
            {
                throw new ArgumentNullException("fileParser");
            }

           Repository = fileParser.Parse().Select(ConstructRepository);
        }

        private  IEnumerable<T> Repository
        {
           get; set; 
        }

        /// <see cref="IRepository{T}.GetAll"/>
        public IEnumerable<T> GetAll()
        {
            return Repository;
        }

        /// <summary>
        /// Construct Repository is decided by the sub-classes.
        /// </summary>
        public abstract T ConstructRepository(IReadOnlyList<string> data);
    }
}