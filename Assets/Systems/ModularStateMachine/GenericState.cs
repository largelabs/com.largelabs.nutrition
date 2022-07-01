using System.Collections.Generic;
using UnityEngine;

public class GenericState : State
{
    [SerializeField] private string genericStateId;

    [SerializeField] List<Action> initialActions;
    [SerializeField] List<Action> entryActions;
    [SerializeField] List<Action> updateActions;
    [SerializeField] List<Action> fixedUpdateActions;
    [SerializeField] List<Action> exitActions;

    public string GenericStateId { get { return genericStateId; }}

    

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

    protected override void onStateFixedUpdate()
    {
        if (fixedUpdateActions is null)
            return;

        foreach (var action in fixedUpdateActions)
        {
            action.PerformAction();
        }
    }
}
