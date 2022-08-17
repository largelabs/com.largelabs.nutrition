using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public abstract class State : MonoBehaviourBase
{
    private StateMachine stateMachine = null;
    public Controls controls = null;
    private bool isInit = false;
    private StateFeatureAbstract[] features;

    #region PUBLIC API
    public void Initialize(StateMachine i_stateMachine, Controls i_controls )
    {
        if (isInit) return;

        stateMachine = i_stateMachine;
        controls = i_controls;
        features = GetComponentsInChildren<StateFeatureAbstract>();
        onStateInit();
        foreach ( StateFeatureAbstract feature in features) feature.FeatureInit();

        isInit = true;
    }

    public void EnterState()
    {
        onStateEnter();
        foreach ( StateFeatureAbstract feature in features) feature.FeatureStart();
    }
    public void UpdateState()
    {
        onStateUpdate();
        foreach (StateFeatureAbstract feature in features) feature.FeatureUpdate();
    }

    public void FixedUpdateState()
    {
        onStateFixedUpdate();
        foreach (StateFeatureAbstract feature in features) feature.FeatureFixedUpdate();
    }

    public void ExitState()
    {
        onStateExit();
        foreach ( StateFeatureAbstract feature in features) feature.FeatureExit();
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