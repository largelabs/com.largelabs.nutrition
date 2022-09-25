using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoraInputs : MonoBehaviourBase
{
    [SerializeField] float moveDispatchInterval = 0.2f;
    [SerializeField] float eatDispatchInterval = 0.25f;

    DoraActions inputActions = null;

    public Action OnEatStarted = null;
    public Action OnEat = null;
    public Action OnEatReleased = null;

    public Action<Vector2> OnMoveStarted = null;
    public Action<Vector2> OnMove = null;
    public Action<Vector2> OnMoveReleased = null;

    Coroutine moveRoutine = null;
    Coroutine eatRoutine = null;

    #region UNITY AND CORE

    protected override void Awake()
    {
        base.Awake();
        initInputs();
    }

    private void OnDestroy()
    {
        DisableInputs();

        OnEatStarted = null;
        OnEat = null;
        OnEatReleased = null;

        OnMoveStarted = null;
        OnMove = null;
        OnMoveReleased = null;

        unregisterInputs();
    }

    #endregion

    #region PUBLIC API

    public void EnableInputs()
    {
        EnableMoveInputs();
        EnableEatInputs();
    }

    public void EnableMoveInputs()
    {
        if (null != inputActions) inputActions.Player.Move.Enable();
    }

    public void EnableEatInputs()
    {
        if (null != inputActions) inputActions.Player.Eat.Enable();
    }

    public void DisableMoveInputs()
    {
        if(null != inputActions) inputActions.Player.Move.Disable();
        this.DisposeCoroutine(ref moveRoutine);
    }

    public void DisableEatInputs()
    {
        if (null != inputActions) inputActions.Player.Eat.Disable();
        this.DisposeCoroutine(ref eatRoutine);
    }

    public void DisableInputs()
    {
        DisableMoveInputs();
        DisableEatInputs();
    }

    #endregion

    #region PRIVATE

    private IEnumerator dispatchMove()
    {
        OnMoveStarted?.Invoke(inputActions.Player.Move.ReadValue<Vector2>());

        while (true)
        {
            yield return moveDispatchInterval <= 0f ? null : this.Wait(moveDispatchInterval);
            OnMove?.Invoke(inputActions.Player.Move.ReadValue<Vector2>());
        }
    }

   /* void disposeRoutines()
    {
        this.DisposeCoroutine(ref moveRoutine);
        this.DisposeCoroutine(ref eatRoutine);
    } */

    private IEnumerator dispatchEat()
    {
        OnEatStarted?.Invoke();
        yield return eatDispatchInterval <= 0f ? null : this.Wait(eatDispatchInterval);

        while (true)
        {
            OnEat?.Invoke();
            yield return eatDispatchInterval <= 0f ? null : this.Wait(eatDispatchInterval);
        }
    }

    private void onMoveStarted(InputAction.CallbackContext obj)
    {
        if(null == moveRoutine) moveRoutine = StartCoroutine(dispatchMove());
    }

    private void onMoveCanceled(InputAction.CallbackContext obj)
    {
        this.DisposeCoroutine(ref moveRoutine);
        OnMoveReleased?.Invoke(MathConstants.VECTOR_2_ZERO);
    }

    private void onEatStarted(InputAction.CallbackContext obj)
    {
        this.DisposeCoroutine(ref eatRoutine);
        eatRoutine = StartCoroutine(dispatchEat());
    }

    private void onEatCanceled(InputAction.CallbackContext obj)
    {
        this.DisposeCoroutine(ref eatRoutine);
        OnEatReleased?.Invoke();
    }

    void initInputs()
    {
        if (null == inputActions) inputActions = new DoraActions();
        inputActions.Player.Move.started += onMoveStarted;
        inputActions.Player.Move.canceled += onMoveCanceled;

        inputActions.Player.Eat.started += onEatStarted;
        inputActions.Player.Eat.canceled += onEatCanceled;
    }

    void unregisterInputs()
    {
        if (null == inputActions) return;

        inputActions.Player.Move.started -= onMoveStarted;
        inputActions.Player.Move.canceled -= onMoveCanceled;

        inputActions.Player.Eat.started -= onEatStarted;
        inputActions.Player.Eat.canceled -= onEatCanceled;
    }
    #endregion
}
