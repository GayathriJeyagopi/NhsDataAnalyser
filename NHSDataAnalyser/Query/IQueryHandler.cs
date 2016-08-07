namespace NHSDataAnalyser.Query
{
    /// <summary>
    /// Interface for Query Handler.
    /// </summary>
    /// <typeparam name="TQuery"><see cref="IQuery"/></typeparam>
    /// <typeparam name="TResult"><see cref="QueryResult{T}"/></typeparam>
    public interface IQueryHandler<in TQuery, out TResult>
    {
        /// <summary>
        /// Executes the given query and returns the specific <see cref="QueryResult{T}"/>
        /// </summary>
        TResult Execute(TQuery query);
    }
}