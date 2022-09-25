//This is the class for handling inputs so that states can recieve simple events for their logic

using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controls : MonoBehaviourBase
{
    HarankashInputActions inputActions;
    public Action JumpPressed;
    public Action JumpReleased;
    public Action MoveStarted;
    public Action MoveReleased;

    bool locked = false;

    protected override void Awake()
    {
        base.Awake();
        inputActions = new HarankashInputActions();
    }

    private void OnEnable()
    {
        EnableControls();
    }

    private void OnDisable()
    {
        DisableControls();
    }

    public void EnableControls()
    {
        if (locked) return;
        //Debug.LogError("Enable Controls");
        inputActions.Player.HMovement.Enable();
        inputActions.Player.Jump.Enable();
        inputActions.Player.HMovement.performed += (InputAction.CallbackContext obj) => MoveStarted?.Invoke();
        inputActions.Player.HMovement.canceled += (InputAction.CallbackContext obj) => MoveReleased?.Invoke();
        inputActions.Player.Jump.performed += (InputAction.CallbackContext obj) => JumpPressed?.Invoke();
        inputActions.Player.Jump.canceled += (InputAction.CallbackContext obj) => JumpReleased?.Invoke();
    }

    public void DisableControls()
    {
        if (locked) return;
        //Debug.LogError("Disable Controls");
        inputActions.Player.HMovement.Disable();
        inputActions.Player.Jump.Disable();
    }

    public void SetLock(bool i_lock)
    {
        locked = i_lock;
    }

    public float MoveDirection()
    {
        return (inputActions.Player.HMovement.enabled)? inputActions.Player.HMovement.ReadValue<Vector2>().x:0f;
    }

    
}
