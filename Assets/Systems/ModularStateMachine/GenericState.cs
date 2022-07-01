using System.Collections.Generic;
using UnityEngine;

public class GenericState : State
{
    [SerializeField] private string generciStateId = null;

    public List<Action> initialActions;
    public List<Action> entryActions;
    public List<Action> updateActions;
    public List<Action> exitActions;
    public List<Action> transitionActions;

    // ?????
    public void Transition()
    {
        foreach (var action in transitionActions)
        {
            action.PerformAction();
        }
    }

    protected override void onStateInit()
    {
        if (initialActions is null)
            return;

        foreach (var action in initialActions)
        {
            action.PerformAction();
        }
    }

    protected override void onStateEnter()
    {
        if (entryActions is null)
            return;

        foreach (var action in entryActions)
        {
            action.PerformAction();
        }
    }

    protected override void onStateExit()
    {
        if (exitActions is null)
            return;

        foreach (var action in exitActions)
        {
            action.PerformAction();
        }
    }

    protected override void onStateUpdate()
    {
        if (updateActions is null)
            return;

        foreach (var action in updateActions)
        {
            action.PerformAction();
        }
    }
}
