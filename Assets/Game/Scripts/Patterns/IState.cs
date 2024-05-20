namespace Game.Scripts.Patterns
{
    public interface IState<T>
    {
        void OnStateEnter(T state) {}
        void OnUpdate(T state) {}
        void OnStateResume(T state) {}
        void OnStateExit(T state) {}
    }
}