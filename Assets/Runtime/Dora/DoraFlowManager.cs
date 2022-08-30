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

    [SerializeField] private MinigameTimer timer = null;

    [Header("Extra Options")]
    [SerializeField] [Range(1, 4)] private int doraPerBatch = 4;

    private List<DoraPlacer.DoraPositions> doraPositions = new List<DoraPlacer.DoraPositions>
                { DoraPlacer.DoraPositions.BackLeft, DoraPlacer.DoraPositions.BackRight,
                    DoraPlacer.DoraPositions.FrontLeft, DoraPlacer.DoraPositions.FrontRight};

    private DoraCellMap currentCob = null;
    private DoraCellMap previousCob = null;

    Coroutine doraGameplayRoutine;
    Coroutine burntDoraRoutine;

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
        int length = Mathf.Clamp(doraPerBatch, 1, 4);
        for (int i = 0; i < length; i++)
        {
            currCob = doraPlacer.SpawnDoraAtAnchor(doraPositions[i]);
            currCob.InitializeDoraCob();
            yield return this.Wait(1.0f);
        }
        //currCob = doraPlacer.SpawnDoraAtAnchor(DoraPlacer.DoraPositions.BackLeft);
        //currCob.InitializeDoraCob();
        //yield return this.Wait(1.0f);

        //currCob = doraPlacer.SpawnDoraAtAnchor(DoraPlacer.DoraPositions.BackRight);
        //currCob.InitializeDoraCob();
        //yield return this.Wait(1.0f);

        //currCob = doraPlacer.SpawnDoraAtAnchor(DoraPlacer.DoraPositions.FrontLeft);
        //currCob.InitializeDoraCob();
        //yield return this.Wait(1.0f);

        //currCob = doraPlacer.SpawnDoraAtAnchor(DoraPlacer.DoraPositions.FrontRight);
        //currCob.InitializeDoraCob();
        //yield return this.Wait(3.4f);


        yield return this.Wait(2.4f);

        doraMover.reverseQueue();
    }


    protected override void onGameplayStarted()
    {
        Debug.LogError("on gameplay started");

        registerEvents();

        timer.StartTimer();

        startDoraFlow();
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

    private IEnumerator burntDoraSequence()
    {
        // play some burnt dora feedback
        yield return null;
        doraMover.GetNextCob();
        this.DisposeCoroutine(ref burntDoraRoutine);
    }

    private IEnumerator doraBatchSequence()
    {
        timer.PauseTimer();

        yield return StartCoroutine(introRoutine());

        timer.ResumeTimer();

        startDoraFlow();
    }

    private void goToSuccess()
    {
        StartCoroutine(onSuccess());
    }

    private void getNextDoraBatch()
    {
        StartCoroutine(doraBatchSequence());
    }

    private void startDoraFlow()
    {
        IReadOnlyList<DoraCellMap> doraCobs = doraSpawner.LivingDora;
        DoraDurabilityManager currDurabilityManager = null;
        foreach (DoraCellMap doraCob in doraCobs)
        {
            currDurabilityManager = doraCob.GetComponent<DoraDurabilityManager>();

            if (currDurabilityManager != null)
                currDurabilityManager.ActivateDurabilityUpdate();
        }

        doraMover.GetNextCob();
    }

    private void tryStartDoraGameplay(DoraCellMap i_cellMap)
    {
        if (i_cellMap.IsPastBurnThreshold)
        {
            if (burntDoraRoutine == null)
                burntDoraRoutine = StartCoroutine(burntDoraSequence());
            else
                Debug.LogError("The burnt dora sequence already active; " +
                                "there is an issue with the game's flow!");
        }
        else
        {
            if (doraGameplayRoutine == null)
                doraGameplayRoutine = StartCoroutine(doraGameplay(i_cellMap));
            else
                Debug.LogError("The dora gameplay sequence is already active; " +
                                "there is an issue with the game's flow!");
        }
    }

    private void registerEvents()
    {
        doraMover.OnGetNextCob += tryStartDoraGameplay;
        doraMover.OnQueueEmpty += getNextDoraBatch;

        timer.OnTimerEnded += goToSuccess;

    }

    private void unregisterEvents()
    {
        doraMover.OnGetNextCob -= tryStartDoraGameplay;
        doraMover.OnQueueEmpty -= getNextDoraBatch;

        timer.OnTimerEnded -= goToSuccess;

    }

    #endregion
}