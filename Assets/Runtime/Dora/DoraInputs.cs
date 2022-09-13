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

    #endregion

    #region PUBLIC API

    public void EnableInputs()
    {
        inputActions.Player.Move.Enable();
        inputActions.Player.Eat.Enable();
    }
    public void EnableMoveInputs()
    {
        inputActions.Player.Move.Enable();
    }

    public void DisableInputs()
    {
        inputActions.Player.Move.Disable();
        inputActions.Player.Eat.Disable();

        this.DisposeCoroutine(ref moveRoutine);
        this.DisposeCoroutine(ref eatRoutine);
    }
    public void DisableMoveInputs()
    {
        inputActions.Player.Move.Disable();

        this.DisposeCoroutine(ref moveRoutine);
    }

    #endregion

    #region PRIVATE

    private IEnumerator dispatchMove()
    {
        while(true)
        {
            yield return moveDispatchInterval <= 0f ? null : this.Wait(moveDispatchInterval);
            OnMove?.Invoke(inputActions.Player.Move.ReadValue<Vector2>());
        }
    }

    void disposeRoutines()
    {
        this.DisposeCoroutine(ref moveRoutine);
        this.DisposeCoroutine(ref eatRoutine);
    }

    private IEnumerator dispatchEat()
    {
        yield return eatDispatchInterval <= 0f ? null : this.Wait(eatDispatchInterval);

        while (true)
        {
            OnEat?.Invoke();
            yield return eatDispatchInterval <= 0f ? null : this.Wait(eatDispatchInterval);
        }
    }

    private void onMoveStarted(InputAction.CallbackContext obj)
    {
        disposeRoutines();
        //inputActions.Player.Eat.Disable();
        OnMoveStarted?.Invoke(obj.ReadValue<Vector2>());
        moveRoutine = StartCoroutine(dispatchMove());
    }

    private void onMoveCanceled(InputAction.CallbackContext obj)
    {
        disposeRoutines();
        inputActions.Player.Eat.Enable();
        OnMoveReleased?.Invoke(MathConstants.VECTOR_2_ZERO);
    }

    private void onEatStarted(InputAction.CallbackContext obj)
    {
        disposeRoutines();
        inputActions.Player.Move.Disable();
        OnEatStarted?.Invoke();
        eatRoutine = StartCoroutine(dispatchEat());
    }

    private void onEatCanceled(InputAction.CallbackContext obj)
    {
        disposeRoutines();
        inputActions.Player.Move.Enable();
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

    #endregion
}
