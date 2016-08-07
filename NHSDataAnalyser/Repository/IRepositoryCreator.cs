namespace NHSDataAnalyser.Repository
{
    internal interface IRepositoryCreator<out T> where T : class
    {
        T Create();
    }
}