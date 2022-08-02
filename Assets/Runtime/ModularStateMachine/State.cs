using UnityEngine;


public abstract class State : MonoBehaviourBase
{
    private StateMachine stateMachine = null;
    public Controls controls = null;
    private bool isInit = false;

    #region PUBLIC API
    public void Initialize(StateMachine i_stateMachine, Controls i_controls )
    {
        if (isInit) return;

        stateMachine = i_stateMachine;
        controls = i_controls;
        onStateInit();

        isInit = true;
    }

    public void EnterState()
    {
        onStateEnter();
    }
    public void UpdateState()
    {
        onStateUpdate();
    }

    public void FixedUpdateState()
    {
        onStateFixedUpdate();
    }

    public void ExitState()
    {
        onStateExit();
    }

    #endregion

    #region PROTECTED API

    protected void setState(string i_id)
    {
        stateMachine.SetGenericState(i_id);
    }

    protected void setState<TState>() where TState : State
    {
        stateMachine.SetState<TState>();
    }

    protected abstract void onStateInit();

    protected abstract void onStateEnter();

    protected abstract void onStateExit();

    protected abstract void onStateUpdate();

    protected virtual void onStateFixedUpdate() { }

    #endregion

}