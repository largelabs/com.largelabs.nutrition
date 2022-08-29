using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoraFlowManager : MiniGameFlow
{
    [SerializeField] private DoraController doraController = null;
    [SerializeField] private DoraPlacer doraPlacer = null;
    [SerializeField] private DoraMover doraMover = null;
    [SerializeField] private DoraSpawner doraSpawner = null;

    private DoraCellMap currentCob = null;
    private DoraCellMap previousCob = null;

    Coroutine doraGameplayRoutine;

    DoraActions inputActions = null;

    #region Start?
    private void Start()
    {
        EnterMiniGame();
    }

    #endregion

    #region GameFlow
    protected override IEnumerator introRoutine()
    {
        Debug.LogError("intro");
        yield return this.Wait(1.0f);

        DoraCellMap currCob = null;

        currCob = doraPlacer.SpawnDoraAtAnchor(DoraPlacer.DoraPositions.BackLeft);
        currCob.InitializeDoraCob();
        yield return this.Wait(1.0f);

        currCob = doraPlacer.SpawnDoraAtAnchor(DoraPlacer.DoraPositions.BackRight);
        currCob.InitializeDoraCob();
        yield return this.Wait(1.0f);

        currCob = doraPlacer.SpawnDoraAtAnchor(DoraPlacer.DoraPositions.FrontLeft);
        currCob.InitializeDoraCob();
        yield return this.Wait(1.0f);

        currCob = doraPlacer.SpawnDoraAtAnchor(DoraPlacer.DoraPositions.FrontRight);
        currCob.InitializeDoraCob();
        yield return this.Wait(3.4f);

        doraMover.reverseQueue();
    }


    protected override void onGameplayStarted()
    {
        Debug.LogError("on gameplay started");

        registerEvents();

        IReadOnlyList<DoraCellMap> doraCobs = doraSpawner.LivingDora;
        DoraDurabilityManager currDurabilityManager = null;
        foreach (DoraCellMap doraCob in doraCobs)
        {
            currDurabilityManager = doraCob.GetComponent<DoraDurabilityManager>();

            if (currDurabilityManager != null)
                currDurabilityManager.ActivateDurabilityUpdate();
        }

        doraMover.GetNextCob();
        //StartCoroutine(simulatedFlow());
    }

    protected override void onGameplayUpdate()
    {

    }

    protected override void onGameplayEnded()
    {
        unregisterEvents();
    }

    protected override IEnumerator onSuccess()
    {
        Debug.LogError("SUCCESS!");
        yield return null;
    }

    protected override IEnumerator onFailure()
    {
        yield break;
    }
    #endregion

    #region private
    private IEnumerator simulatedFlow()
    {
        doraMover.GetNextCob();
        yield return this.Wait(5f);

        doraMover.GetNextCob();
        yield return this.Wait(5f);

        doraMover.GetNextCob();
        yield return this.Wait(5f); ;

        doraMover.GetNextCob();
        yield return this.Wait(5f);
    }

    private IEnumerator doraGameplay(DoraCellMap i_cellMap)
    {
        if (null == inputActions) inputActions = new DoraActions();

        inputActions.Player.TestAction.Enable();

        doraController.SetCellMap(i_cellMap);

        while (true)
        {
            // gameplay stuff

            if (doraController.CurrentEatenKernelCount == 132)
            {
                doraMover.GetNextCob();
                this.DisposeCoroutine(ref doraGameplayRoutine);
                yield break;
            }

            if (inputActions.Player.TestAction.WasPressedThisFrame())
            {
                doraMover.GetNextCob();
                this.DisposeCoroutine(ref doraGameplayRoutine);
                yield break;
            }

            yield return null;
        }
    }

    private void goToSuccess()
    {
        StartCoroutine(onSuccess());
    }

    private void startDoraGameplay(DoraCellMap i_cellMap)
    {
        if(doraGameplayRoutine == null)
            doraGameplayRoutine = StartCoroutine(doraGameplay(i_cellMap));
    }

    private void registerEvents()
    {
        doraMover.OnGetNextCob += startDoraGameplay;
        doraMover.OnQueueEmpty += goToSuccess;
    }

    private void unregisterEvents()
    {
        doraMover.OnGetNextCob -= startDoraGameplay;
        doraMover.OnQueueEmpty -= goToSuccess;
    }

    #endregion
}
