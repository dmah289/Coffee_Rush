namespace BaseSystem
{
    public interface IPooledObject
    {
        void OnRelease();
        void OnRevoke();
    }
}