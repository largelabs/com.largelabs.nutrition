using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviourBase
{
    [SerializeField] State initialState;
    [SerializeField] Controls controls;
    State currentState;

    State[] allStates = null;

    Dictionary<System.Type, State> allStatesByType = null;
    Dictionary<string, GenericState> allGenericStates = null;

    protected override void Awake()
    {
        base.Awake();
        initializeStateCollections();
    }

    void Start()
    {
        SetState(initialState);
    }

    private void Update()
    {
        if (currentState is null)
            return;

        //Debug.LogError("Current State: " + currentState);

        currentState.UpdateState();
    }

    private void FixedUpdate()
    {
        if (currentState is null)
            return;

        currentState.FixedUpdateState();
    }

    public void SetState(State i_state)
    {
        if (null == i_state) return;

        if (currentState is not null)
            currentState.ExitState();

        currentState = i_state;
        Debug.LogError("Entered state: " + currentState);
        currentState.Initialize(this, controls);
        currentState.EnterState();
    }

    // @Karim : add string as a possible input
    [ExposePublicMethod]
    public void SetGenericState(string i_id)
    {
        if (string.IsNullOrEmpty(i_id)) return;
        State genericState = allGenericStates[i_id];
        SetState(genericState);
    }


    public void SetState<TState>() where TState : State
    {
        System.Type stateType = typeof(TState);

        if (stateType == typeof(GenericState)) return;

        State state = null;
        if (true == allStatesByType.TryGetValue(stateType, out state))
        {
            SetState(state);
        }
    }

    void initializeStateCollections()
    {
        allStates = GetComponentsInChildren<State>(true);
        allStatesByType = new Dictionary<System.Type, State>();
        allGenericStates = new Dictionary<string, GenericState>();

        System.Type currentType = null;
        System.Type genericStateType = typeof(GenericState);

        foreach (State state in allStates)
        {
            currentType = state.GetType();

            if (currentType != genericStateType)
                allStatesByType.Add(currentType, state);
            else
            {
                GenericState genericState = state as GenericState;
                allGenericStates.Add(genericState.GenericStateId, genericState);
            }
        }
    }
}
