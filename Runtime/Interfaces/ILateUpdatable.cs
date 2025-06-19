namespace LiteBindDI
{
    public interface ILateUpdatable : IMonoBase
    {
        void LateTick();
    }
}
