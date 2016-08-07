namespace NHSDataAnalyser.Query
{
    public class QueryResult<T>
    {
        public enum ResultState
        {
            Pass,
            Fail
        }

        public T Result;
        public ResultState State;
    }
}