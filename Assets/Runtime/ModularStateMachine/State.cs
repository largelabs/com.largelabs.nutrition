
public abstract class State : MonoBehaviourBase
{
    private StateMachine stateMachine = null;
    public Controls controls = null;
    private bool isInit = false;
    private StateFeatureAbstract[] features = null;

    #region PUBLIC API
    public void Initialize(StateMachine i_stateMachine, Controls i_controls )
    {
        if (isInit) return;

        stateMachine = i_stateMachine;
        controls = i_controls;

        features = GetComponentsInChildren<StateFeatureAbstract>(true);
        foreach (StateFeatureAbstract feature in features) feature.FeatureInit();

        onStateInit();

        isInit = true;
    }

    public void EnterState()
    {
        foreach (StateFeatureAbstract feature in features) feature.FeatureStart();
        onStateEnter();
    }
    public void UpdateState()
    {
        foreach (StateFeatureAbstract feature in features) feature.FeatureUpdate();
        onStateUpdate();
    }

    public void FixedUpdateState()
    {
        foreach (StateFeatureAbstract feature in features) feature.FeatureFixedUpdate();
        onStateFixedUpdate();
    }

    public void ExitState()
    {
        foreach (StateFeatureAbstract feature in features) feature.FeatureExit();
        onStateExit();
    }

    public abstract void ResetState();

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