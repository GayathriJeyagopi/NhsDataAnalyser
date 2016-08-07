using System.Collections.Generic;

namespace NHSDataAnalyser.Repository
{
    /// <summary>
    ///     Interface for Querying the repository, the intention is to keep the Repository unaware of domain specific queries.
    /// </summary>
    /// <typeparam name="T">T is a type which extends <see cref="AbstractRepository{T}" /></typeparam>
    public interface IRepository<out T>
    {
        /// <summary>
        ///     Gets all items from the repository.
        /// </summary>
        /// <returns>T template type can be any repository class type.</returns>
        IEnumerable<T> GetAll();
    }
}