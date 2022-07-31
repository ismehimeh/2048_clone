using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class InputManager : Singleton<InputManager>
{

    #region Events
    public delegate void StartTouch(Vector2 position);
    public event StartTouch OnStartTouch;
    #endregion

    private TouchControls touchControls;
    private Camera mainCamera;

    private void Awake()
    {
        touchControls = new TouchControls();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        touchControls.Enable();
    }

    private void OnDisable()
    {
        touchControls.Disable();
    }

    private void Start()
    {
        
        touchControls.Touch.PrimaryContact.started += ctx => StartTouchPrimary();
    }

    private void StartTouchPrimary()
    {
        CheckMainCamera();
        if (OnStartTouch != null) OnStartTouch(Utils.ScreenToWorld(mainCamera,
                                                                    touchControls.Touch.PrimaryPosition.ReadValue<Vector2>()));

    }

    public Vector2 PrimaryPosition()
    {
        CheckMainCamera();
        return Utils.ScreenToWorld(mainCamera, touchControls.Touch.PrimaryPosition.ReadValue<Vector2>());
    }

    // for some reason mainCamera is equall null when you load first scene again after loosing
    // idk why, maybe bcause of DefaultExecutionOrder or Singleton
    private void CheckMainCamera()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }
}
