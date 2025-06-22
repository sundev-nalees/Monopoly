using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameEvent<T> : IGameEvent<T>
{
    private event Action<T> listeners;

    public void Subscribe(Action<T> listener)
    {
        listeners += listener;
    }

    public void Unsubscribe(Action<T> listener)
    {
        listeners -= listener;
    }

    public void Raise(T data)
    {
        listeners?.Invoke(data);
    }


}
