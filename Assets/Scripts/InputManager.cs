using UnityEngine;
using System;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerController controls;
    private bool isInitialized = false;

    // Eventos de Player
    public event Action<Vector2> OnMove;
    public event Action<Vector2> OnCameraMove;
    public event Action<float> OnCameraZoom;
    public event Action OnRecenterCamera;
    public event Action OnInteract;

    public event Action OnShoot;
    public event Action OnNextWeapon;
    public event Action OnReload;

    public Vector2 MoveInput { get; private set; }
    public Vector2 AimInput { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        controls = new PlayerController();
        InitializeControls();
    }

    void InitializeControls()
    {
        controls = new PlayerController();

        // Values
        controls.Player.Move.performed += ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
        controls.Player.Move.canceled += ctx => OnMove?.Invoke(Vector2.zero);
        controls.Player.CameraMove.performed += ctx => OnCameraMove?.Invoke(ctx.ReadValue<Vector2>());
        controls.Player.CameraMove.canceled += ctx => OnCameraMove?.Invoke(Vector2.zero);
        controls.Player.Zoom.performed += ctx => OnCameraZoom?.Invoke(ctx.ReadValue<float>());
        controls.Player.Zoom.canceled += ctx => OnCameraZoom?.Invoke(0);

        // Buttons
        controls.Player.RecenterCamera.performed += _ => OnRecenterCamera?.Invoke();
        controls.Player.Interact.performed += _ => OnInteract?.Invoke();

        controls.Player.Enable();
        isInitialized = true;
    }

    void OnEnable()
    {
        if (isInitialized && controls != null)
        {
            controls.Player.Enable();
        }
        controls?.Enable();
        
    }

    void OnDisable()
    {
        if (controls != null)
        {
            controls.Player.Disable();
            controls.Player.Interact.performed -= ctx => OnInteract?.Invoke();
            controls.Disable();
        }
    }

    void OnDestroy()
    {
        if (controls != null)
        {
            controls.Dispose();
            controls = null;
        }
    }
}