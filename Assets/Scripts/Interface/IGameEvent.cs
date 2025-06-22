using System;


public interface IGameEvent<T>
{
    public void Subscribe(Action<T> listener);
    public void Unsubscribe(Action<T> listener);
    public void Raise(T data);
}
