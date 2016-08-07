namespace NHSDataAnalyser.Repository
{
    /// <summary>
    /// Creates Repository of Type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal interface IRepositoryCreator<out T> where T : class
    {
        /// <summary>
        /// Creates a new Repository of Type T.
        /// </summary>
       T Create();
    }
}