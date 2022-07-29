//This is the class for handling inputs so that states can recieve simple events for their logic

using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class HarankashControls : MonoBehaviourBase
{
    HarankashInputActions inputActions;
    public Action JumpPressed;
    public Action JumpReleased;
    public Action MoveStarted;
    public Action MoveReleased;

    protected override void Awake()
    {
        base.Awake();
        inputActions = new HarankashInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.HMovement.Enable();
        inputActions.Player.Jump.Enable();
        inputActions.Player.HMovement.performed += (InputAction.CallbackContext obj) => MoveStarted?.Invoke();
        inputActions.Player.HMovement.canceled += (InputAction.CallbackContext obj) => MoveReleased?.Invoke(); 
        inputActions.Player.Jump.performed += (InputAction.CallbackContext obj) => JumpPressed?.Invoke();
        inputActions.Player.Jump.canceled += (InputAction.CallbackContext obj) => JumpReleased?.Invoke();
    }

    private void OnDisable()
    {
        inputActions.Player.HMovement.Disable();
        inputActions.Player.Jump.Disable();
        
    }

    public float MoveDirection()
    {
        return inputActions.Player.HMovement.ReadValue<float>();
    }
}
