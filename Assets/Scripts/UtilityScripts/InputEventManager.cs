using System;
using UnityEngine;

public class InputEventManager : MonoBehaviour
{
    public static InputEventManager Singleton { get; private set; }
    public InputSystem Inputsystem;
    private void Awake()
    {
        Singleton = this;
        Inputsystem = new InputSystem();
        Inputsystem.Tank.Enable();
        DontDestroyOnLoad(gameObject);
    }
}
