using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public State initialState;
    State currentState;
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

    public void SetState(State newState)
    {
        if (currentState is not null)
            currentState.ExitState();

        currentState = newState;
        currentState.EnterState();
    }
    private void LateUpdate()
    {
        currentState.Transition();
    }
}
