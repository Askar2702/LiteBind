namespace LiteBindDI
{
    public interface IFactory<TParam, TResult>
    {
        TResult Create(TParam param);
    }
}
