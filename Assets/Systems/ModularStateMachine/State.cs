using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State : MonoBehaviour
{
    public List<Action> initialActions;
    public List<Action> entryActions;
    public List<Action> updateActions;
    public List<Action> exitActions;
    public List<Action> transitionActions;

    private void Start()
    {
        Initialize();
    }
    public void Initialize()
    {
        if (initialActions is null)
            return;

        foreach (var action in initialActions)
        {
            action.performAction();
        }
    }
    public void EnterState()
    {
        if (entryActions is null)
            return;

        foreach (var action in entryActions)
        {
            action.performAction();
        }
    }
    public void UpdateState()
    {
        if (updateActions is null)
            return;

        foreach (var action in updateActions)
        {
            action.performAction();
        }
    }
    public void ExitState()
    {
        if (exitActions is null)
            return;

        foreach (var action in exitActions)
        {
            action.performAction();
        }
    }
    public void Transition()
    {
        foreach (var action in transitionActions)
        {
            action.performAction();
        }
    }
}
