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

    // Mouse
    public event Action onLeftMouseButtonDown;
    public event Action onLeftMouseButtonUp;
    public event Action onLeftMouseButtonHold;
    public event Action onRightMouseButtonDown;
    public event Action onRightMouseButtonUp;
    public event Action onRightMouseButtonHold;

    // Keyboard
    public event Action onSpacePressDown;
    public event Action onSpacePressUp;
    public event Action onEscPressDown;
    public event Action onEscPressUp;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            onLeftMouseButtonDown?.Invoke();

        if (Input.GetMouseButtonUp(0))
            onLeftMouseButtonUp?.Invoke();

        if (Input.GetMouseButton(0))
            onLeftMouseButtonHold?.Invoke();

        if (Input.GetMouseButtonDown(1))
            onRightMouseButtonDown?.Invoke();

        if (Input.GetMouseButtonUp(1))
            onRightMouseButtonUp?.Invoke();

        if (Input.GetMouseButton(1))
            onRightMouseButtonHold?.Invoke();

        if (Input.GetKeyDown(KeyCode.Space))
            onSpacePressDown?.Invoke();

        if (Input.GetKeyUp(KeyCode.Space))
            onSpacePressUp?.Invoke();

        if (Input.GetKeyDown(KeyCode.Escape))
            onEscPressDown?.Invoke();

        if (Input.GetKeyUp(KeyCode.Escape))
            onEscPressUp?.Invoke();
    }
}
