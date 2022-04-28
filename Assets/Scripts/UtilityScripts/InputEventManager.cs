using System;
using UnityEngine;

public class InputEventManager : MonoBehaviour
{
    public static InputEventManager Singleton { get; private set; }
    private void Awake()
    {
        Singleton = this;

        DontDestroyOnLoad(gameObject);
    }
}
