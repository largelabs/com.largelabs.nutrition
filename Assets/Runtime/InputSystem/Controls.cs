//This is the class for handling inputs so that states can recieve simple events for their logic

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controls : MonoBehaviourBase
{
    [SerializeField] float moveDispatchInterval = 0.2f;

    HarankashInputActions inputActions;
    public Action JumpPressed;
    public Action JumpReleased;
    public Action MoveStarted;
    public Action Move;
    public Action MoveReleased;

    bool locked = false;
    Coroutine moveRoutine = null;

    protected override void Awake()
    {
        base.Awake();
        initInputs();
    }

    private void OnEnable()
    {
        EnableControls();
    }

    private void OnDisable()
    {
        DisableControls();
    }

    private void OnDestroy()
    {
        DisableControls();

        JumpPressed = null;
        JumpReleased = null;

        MoveStarted = null;
        Move = null;
        MoveReleased = null;

        unregisterInputs();
    }

    #region PUBLIC API

    public void EnableControls()
    {
        if (locked) return;
        //Debug.LogError("Enable Controls");
        inputActions.Player.HMovement.Enable();
        inputActions.Player.Jump.Enable();
    }

    public void DisableControls()
    {
        if (locked) return;
        //Debug.LogError("Disable Controls");
        inputActions.Player.HMovement.Disable();
        inputActions.Player.Jump.Disable();
        this.DisposeCoroutine(ref moveRoutine);
    }

    public void SetLock(bool i_lock)
    {
        locked = i_lock;
    }

    public float MoveDirection()
    {
        return (inputActions.Player.HMovement.enabled)? inputActions.Player.HMovement.ReadValue<Vector2>().x:0f;
    }

    #endregion

    #region PRIVATE

    void initInputs()
    {
        if (null == inputActions) inputActions = new HarankashInputActions();
        inputActions.Player.HMovement.performed += onMoveStarted;
        inputActions.Player.HMovement.canceled += onMoveCanceled;

        inputActions.Player.Jump.started += onJumpStarted;
        inputActions.Player.Jump.canceled += onJumpCanceled;
    }
    void unregisterInputs()
    {
        if (null == inputActions) return;
        inputActions.Player.HMovement.performed -= onMoveStarted;
        inputActions.Player.HMovement.canceled -= onMoveCanceled;

        inputActions.Player.Jump.started -= onJumpStarted;
        inputActions.Player.Jump.canceled -= onJumpCanceled;
    }

    private void onMoveStarted(InputAction.CallbackContext obj)
    {
        if (null == moveRoutine) moveRoutine = StartCoroutine(dispatchMove());
    }

    private void onMoveCanceled(InputAction.CallbackContext obj)
    {
        this.DisposeCoroutine(ref moveRoutine);
        MoveReleased?.Invoke();
    }

    private void onJumpStarted(InputAction.CallbackContext obj)
    {
        JumpPressed?.Invoke();
    }

    private void onJumpCanceled(InputAction.CallbackContext obj)
    {
        JumpReleased?.Invoke();
    }

    private IEnumerator dispatchMove()
    {
        MoveStarted?.Invoke();

        while (true)
        {
            yield return moveDispatchInterval <= 0f ? null : this.Wait(moveDispatchInterval);
            Move?.Invoke();
        }
    }

    #endregion

}
