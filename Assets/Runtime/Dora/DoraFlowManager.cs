using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoraFlowManager : MiniGameFlow
{
    [SerializeField] private DoraPlacer doraPlacer = null;
    [SerializeField] private DoraMover doraMover = null;
    [SerializeField] private DoraSpawner doraSpawner = null;

    private DoraCellMap currentCob = null;
    private DoraCellMap previousCob = null;

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

        StartCoroutine(simulatedFlow());
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
        yield break;
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

    private void getCurrentCob(DoraCellMap i_cellMap)
    {
        currentCob = i_cellMap;
    }

    private void registerEvents()
    {
        doraMover.OnTryGetNextCob += getCurrentCob;
    }

    private void unregisterEvents()
    {
        doraMover.OnTryGetNextCob -= getCurrentCob;
    }

    #endregion
}
