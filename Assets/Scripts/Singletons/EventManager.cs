using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : Singleton<EventManager>
{
    public UnityEvent GameStart = new();
    public UnityEvent GameWin   = new();

    public UnityEvent PlayerSpawn = new();
    public UnityEvent PlayerDie   = new();

    public UnityEvent LeaveScene = new();
    public UnityEvent EnterScene = new();
}

public class BoolEvent : UnityEvent<bool> { }
