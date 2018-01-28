namespace Logic.Infrastructure.Query
{
    public interface IQueryHandler { }

    public interface IQueryHandler<in TQuery, out TResult> : IQueryHandler where TQuery : IQuery<TResult>
    {
        TResult Execute(TQuery query);
    }
}