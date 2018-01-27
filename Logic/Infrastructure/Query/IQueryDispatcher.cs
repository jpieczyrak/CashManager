namespace Logic.Infrastructure.Query
{
    public interface IQueryDispatcher
    {
        TResult Execute<TQuery, TResult>(TQuery query) where TQuery : IQuery;
    }
}