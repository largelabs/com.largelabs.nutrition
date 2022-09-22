using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoraFlowManager : MiniGameFlow
{
    [SerializeField] private DoraAbstractController doraController = null;
    [SerializeField] private DoraPlacer doraPlacer = null;
    [SerializeField] private DoraMover doraMover = null;
    [SerializeField] private DoraSpawner doraSpawner = null;

    [SerializeField] private DoraScoreManager scoreManager = null;
    [SerializeField] private SpawnPool vfxPool = null;
    [SerializeField] private MinigameTimer timer = null;
    [SerializeField] private BoxCollider cullingBounds = null;
    [SerializeField] private BoxCollider selectionBounds = null;
    [SerializeField] private DoraSFXProvider sfxProvider = null;
    [SerializeField] private PanCamera panCamera = null;


    [Header("Options")]
    [SerializeField] private DoraGameData doraGameData = null;
    [SerializeField] private List<DoraBatchData> doraBatchData = null;

    DoraBatchData currentDoraBatch = null;
    DoraDurabilityManager currentDurabilityManager = null;
    DoraActions inputActions = null;
    List<DoraPlacer.DoraPositions> doraPositions = null;
    int doraBatchCount = 0;

    #region UNITY AND CORE

    private void Start() { EnterMiniGame(); }

    #endregion

    #region PUBLIC API

    [ExposePublicMethod]
    public void RestartFlow()
    {
        disposeAllCoroutines();
        EnterMiniGame();
    }

    #endregion

    #region GAME_FLOW

    protected override IEnumerator introRoutine()
    {
        resetGame();

        bringNewBatch();
        yield return this.Wait(1f);

        panCamera.PanCameraDown();
        while (true == panCamera.IsMovingCamera) yield return null;
    }


    protected override void onGameplayStarted()
    {
        registerEvents();
        timer.StartTimer();
        startDoraFlow();
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

    protected override IEnumerator onFailure() { yield break; }
    #endregion

    #region PRIVATE

    void resetGame()
    {
        if (null == doraPositions) doraPositions = new List<DoraPlacer.DoraPositions>
        {
            DoraPlacer.DoraPositions.BackLeft,
            DoraPlacer.DoraPositions.BackRight,
            DoraPlacer.DoraPositions.FrontLeft,
            DoraPlacer.DoraPositions.FrontRight
        };

        unregisterEvents();

        doraSpawner.DespawnAllDora();
        doraController.DisableController();

        scoreManager.ResetScoreManager();
        doraMover.ResetMover();

        timer.SetTimer(doraGameData.BaseTimer, true);
        currentDurabilityManager = null;
        currentDoraBatch = null;
        doraBatchCount = 0;
    }

    private IEnumerator stopGameplay()
    {
        while (true == doraController.IsEating) yield return null;

        doraController.StopController();
        sfxProvider.StopAmbientSounds();
    }

    void bringNewBatch()
    {
        // Maybe change way of choosing batch?
        currentDoraBatch = doraBatchData[UnityEngine.Random.Range(0, doraBatchData.Count)];

        doraBatchCount++;

        DoraCellMap currCob = null;
        int length = Mathf.Clamp(currentDoraBatch.DoraInBatch, 1, 4);
        bool superKernelSpawned;
        int superKernelCobsSpawned = 0;

        float superKernelChance = currentDoraBatch.SuperKernelChance;

        for (int i = 0; i < length; i++)
        {
            currCob = doraPlacer.SpawnDoraAtAnchor(doraPositions[i]);
            currCob.InitializeDoraCob(vfxPool, cullingBounds, selectionBounds, currentDoraBatch, doraBatchCount, canSpawnSuper(superKernelCobsSpawned, ref superKernelChance), out superKernelSpawned);

            if (superKernelSpawned)
                superKernelCobsSpawned++;
        }
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

    private IEnumerator doraBatchSequence()
    {
        timer.PauseTimer();

        scoreManager.AddScoreByValue(currentDoraBatch.BatchFinishScoreBonus, 
            PopupSpawner.PopupType.Super, Vector3.zero, 1.0f, 0.5f, 10f);

        // maybe animate time increase
        timer.AddTime(currentDoraBatch.BatchFinishTimeBonus);
        sfxProvider.PlayTimeBonusSFX();

        yield return this.Wait(1.0f);

        bringNewBatch();
        yield return this.Wait(1.0f);

        timer.ResumeTimer();

        startDoraFlow();
    }

    private void goToSuccess()
    {
        EndMiniGame(true);
    }

    private void getNextDoraBatch() {  StartCoroutine(doraBatchSequence()); }

    private void startDoraFlow()
    {
        IReadOnlyList<DoraCellMap> doraCobs = doraSpawner.LivingDora;
        DoraDurabilityManager currDurabilityManager = null;
        foreach (DoraCellMap doraCob in doraCobs)
        {
            currDurabilityManager = doraCob.GetComponent<DoraDurabilityManager>();

         //   if (currDurabilityManager != null)
          //      currDurabilityManager.UpdateDurability(true);
        }

        doraMover.GetNextCob();
    }

    void onEat()
    {
        if (doraController.DidEatAllKernels || doraController.GoodKernelsEatenCount == currentDurabilityManager.UnburntKernels)
        {
            currentDurabilityManager = null;
            doraController.DisableController();
            doraMover.GetNextCob();
        }
    }

    private void tryStartDoraGameplay(DoraCellMap i_cellMap, AutoRotator i_autoRotate, DoraDurabilityManager i_durabilityManager)
    {
        if (null == inputActions) inputActions = new DoraActions();
        inputActions.Player.TestAction.Enable();
        doraController.SetDoraComponents(i_cellMap, i_autoRotate, i_durabilityManager.UnburntKernels);
        doraController.EnableController();
        doraController.StartAutoRotation();

        currentDurabilityManager = i_durabilityManager;
    }

    private void registerEvents()
    {
        doraMover.OnGetNextCob += tryStartDoraGameplay;
        doraMover.OnQueueEmpty += getNextDoraBatch;
        timer.OnTimerEnded += goToSuccess;
        doraController.OnDidFinishEating += onEat;
    }

    private void unregisterEvents()
    {
        doraMover.OnGetNextCob -= tryStartDoraGameplay;
        doraMover.OnQueueEmpty -= getNextDoraBatch;
        timer.OnTimerEnded -= goToSuccess;
        doraController.OnDidFinishEating -= onEat;
    }

    #endregion
}