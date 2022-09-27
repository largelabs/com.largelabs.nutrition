using System.Collections.Generic;
using UnityEngine;

public class GenericState : State
{
    [SerializeField] private string genericStateId;

    [SerializeField] List<GenericAction> initialActions;
    [SerializeField] List<GenericAction> entryActions;
    [SerializeField] List<GenericAction> updateActions;
    [SerializeField] List<GenericAction> fixedUpdateActions;
    [SerializeField] List<GenericAction> exitActions;

    public string GenericStateId { get { return genericStateId; } }



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

    public override void ResetState()
    {
       
    }
}
