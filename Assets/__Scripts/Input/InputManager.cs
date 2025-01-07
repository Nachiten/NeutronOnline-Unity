#define USE_NEW_INPUT_SYSTEM

using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    private AutoGen_PlayerInputActions playerInputActions;
    
    protected override void Awake()
    {
        base.Awake();
        
#if USE_NEW_INPUT_SYSTEM
        playerInputActions = new AutoGen_PlayerInputActions();
        playerInputActions.Player.Enable();
#endif
    }

    private void OnDestroy()
    {
#if USE_NEW_INPUT_SYSTEM
        playerInputActions.Player.Disable();
#endif
    }

    public Vector2 GetMouseScreenPosition()
    {
#if USE_NEW_INPUT_SYSTEM
        return MobileBuildCheck.IsMobileBuild() ? 
            Touchscreen.current.primaryTouch.position.ReadValue() :
            Mouse.current.position.ReadValue();
#else
        return Input.mousePosition;
#endif
    }

    public bool WasPrimaryActionReleasedThisFrame()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.PrimaryAction.WasReleasedThisFrame();
#else
        return Input.GetMouseButtonDown(0);
#endif
    }
    
    public bool WasSecondaryActionReleasedThisFrame()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.SecondaryAction.WasReleasedThisFrame();
#else
        return Input.GetMouseButtonDown(1);
#endif
    }
}
