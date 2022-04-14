using System;
using UnityEngine;

public class InputEventManager : MonoBehaviour
{
    public static InputEventManager Singleton { get; private set; }
    private void Awake()
    {
        Singleton = this;

        DontDestroyOnLoad(this.gameObject);
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
            this.onLeftMouseButtonDown?.Invoke();

        if (Input.GetMouseButtonUp(0))
            this.onLeftMouseButtonUp?.Invoke();

        if (Input.GetMouseButton(0))
            this.onLeftMouseButtonHold?.Invoke();

        if (Input.GetMouseButtonDown(1))
            this.onRightMouseButtonDown?.Invoke();

        if (Input.GetMouseButtonUp(1))
            this.onRightMouseButtonUp?.Invoke();

        if (Input.GetMouseButton(1))
            this.onRightMouseButtonHold?.Invoke();

        if (Input.GetKeyDown(KeyCode.Space))
            this.onSpacePressDown?.Invoke();

        if (Input.GetKeyUp(KeyCode.Space))
            this.onSpacePressUp?.Invoke();

        if (Input.GetKeyDown(KeyCode.Escape))
            this.onEscPressDown?.Invoke();

        if (Input.GetKeyUp(KeyCode.Escape))
            this.onEscPressUp?.Invoke();
    }
}
