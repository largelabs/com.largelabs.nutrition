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
    [SerializeField] private GameObject doraHUD = null;
    [SerializeField] private MinigameTimer timer = null;
    [SerializeField] private DoraScoreManager scoreManager = null;

    [Header("Options")]
    [SerializeField] private DoraGameData doraGameData = null;
    [SerializeField] private List<DoraBatchData> doraBatchData = null;

    DoraBatchData currentDoraBatch = null;

    private List<DoraPlacer.DoraPositions> doraPositions = new List<DoraPlacer.DoraPositions>
                { DoraPlacer.DoraPositions.BackLeft, DoraPlacer.DoraPositions.BackRight,
                    DoraPlacer.DoraPositions.FrontLeft, DoraPlacer.DoraPositions.FrontRight};

    private DoraCellMap currentCob = null;
    private DoraCellMap previousCob = null;

    Coroutine doraGameplayRoutine = null;
    Coroutine burntDoraRoutine = null;

    DoraActions inputActions = null;

    #region UNITY AND CORE

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

        yield return StartCoroutine(bringNewBatch());
    }

    protected override void onGameplayStarted()
    {
        Debug.LogError("on gameplay started");

        registerEvents();

        timer.SetTimer(doraGameData.BaseTimer);
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

    #region PRIVATE
    IEnumerator bringNewBatch()
    {
        // Maybe change way of choosing batch?
        currentDoraBatch = doraBatchData[UnityEngine.Random.Range(0, doraBatchData.Count)];

        DoraCellMap currCob = null;
        int length = Mathf.Clamp(currentDoraBatch.DoraInBatch, 1, 4);
        bool superKernelSpawned;
        int superKernelCobsSpawned = 0;

        float superKernelChance = currentDoraBatch.SuperKernelChance;

        for (int i = 0; i < length; i++)
        {
            currCob = doraPlacer.SpawnDoraAtAnchor(doraPositions[i]);
            currCob.InitializeDoraCob(currentDoraBatch, canSpawnSuper(superKernelCobsSpawned, ref superKernelChance), out superKernelSpawned);

            if (superKernelSpawned)
                superKernelCobsSpawned++;
            yield return this.Wait(1.0f);
        }

        doraMover.ReverseQueue();
    }

    private bool canSpawnSuper(int i_superKernelCobsSpawned, ref float i_superKernelChance)
    {
        if (i_superKernelCobsSpawned < currentDoraBatch.MaxSuperKernelsPerBatch)
        {
            if (UnityEngine.Random.Range(0f, 1f) < i_superKernelChance)
                return true;
        }

        i_superKernelChance += currentDoraBatch.SuperKernelChanceIncrease;
        return false;
    }

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
        doraController.EnableController();
        doraController.StartAutoRotation();

        int totalCellCount = i_cellMap.TotalCellCount;

        doraHUD.SetActive(true);

        while (true)
        {
            // gameplay stuff

            if (doraController.CurrentEatenKernelCount == totalCellCount)
            {
                doraHUD.SetActive(false);
                doraMover.GetNextCob();
                doraController.DisableController();
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

        scoreManager.AddScore(currentDoraBatch.BatchFinishScoreBonus);

        // maybe animate time increase
        timer.AddTime(currentDoraBatch.BatchFinishTimeBonus);

        yield return StartCoroutine(bringNewBatch());

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
