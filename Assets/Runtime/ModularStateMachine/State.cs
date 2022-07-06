using UnityEngine;


public abstract class State : MonoBehaviourBase
{
    private StateMachine stateMachine = null;
    private bool isInit = false;

    #region PUBLIC API
    public void Initialize(StateMachine i_stateMachine)
    {
        if (isInit) return;

        stateMachine = i_stateMachine;
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

    protected void setState(GenericState i_state)
    {
        stateMachine.SetState(i_state);
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