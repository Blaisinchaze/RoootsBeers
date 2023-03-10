using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NonPersistantSingleton<T> : MonoBehaviour where T : NonPersistantSingleton<T>
{
    private static T _instance;
    public static T Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = FindObjectOfType<T>();
        }
    }
}
