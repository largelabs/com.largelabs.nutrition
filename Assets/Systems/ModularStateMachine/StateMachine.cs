using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public State initialState;
    State currentState;

    GenericState[] genericStates = null; 
    State[] allStates = null;

    Dictionary<System.Type, State> allStatesByType = null;
    Dictionary<string, GenericState> allGenericStates = null;

    private void Awake()
    {
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

        currentState.UpdateState();
    }

    private void FixedUpdate()
    {
        if (currentState is null)
            return;

        currentState.FixedUpdateState();
    }

    public void SetState(State i_newState)
    {
        if (currentState is not null)
            currentState.ExitState();

        currentState = i_newState;
        currentState.Initialize(this);
        currentState.EnterState();
    }


    public void SetState<TState>() where TState : State
    {
        System.Type stateType = typeof(TState);

        // check that state type isn't generic state

        // Get State in dictionary

        // Call SetState(State)
    }

    void initializeStateCollections()
    {
        allStates = GetComponentsInChildren<State>();

        // Build dictionaries and other collections

    }

}
