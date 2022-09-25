using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoraFlowManager : MiniGameFlow
{
    [Header("Dora flow managers")]
    [SerializeField] private DoraAbstractController doraController = null;
    [SerializeField] private DoraPlacer doraPlacer = null;
    [SerializeField] private DoraMover doraMover = null;
    [SerializeField] private DoraSpawner doraSpawner = null;
    [SerializeField] private DoraScoreManager scoreManager = null;
    [SerializeField] private MinigameTimer timer = null;

    [Header("UI")]
    [SerializeField] private UIDoraEndGamePopup endGamePopup = null;
    [SerializeField] private UIKernelManagerV2 UIkernelManager = null;
    [SerializeField] private GameObject hudGo = null;
    [SerializeField] private PopupSpawner scorePopupSpawner = null;

    [Header("VFX / SFX")]
    [SerializeField] private PanCamera panCamera = null;
    [SerializeField] private SpawnPool vfxPool = null;
    [SerializeField] private DoraSFXProvider sfxProvider = null;

    [Header("Bounds")]
    [SerializeField] private BoxCollider cullingBounds = null;
    [SerializeField] private BoxCollider selectionBounds = null;

    [Header("Decor")]
    [SerializeField] private GameObject charcoalGroup = null;

    [Header("Game Data")]
    [SerializeField] private DoraGameData doraGameData = null;
    [SerializeField] private List<DoraBatchData> doraBatchData = null;
    [SerializeField] private float cobExitTime = 0.2f;
    [SerializeField] private float cobEnterTime = 0.2f;

    // Flow data
    DoraBatchData currentDoraBatch = null;
    Transform currentCob = null;
    DoraDurabilityManager currentDurabilityManager = null;
    int doraBatchCount = 0;
    private Stack<Transform> doraCobStack = null;
    List<DoraPlacer.DoraPositions> doraPositions = null;

    // Routines
    Coroutine getNextCobRoutine = null;

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
        sfxProvider.PlayMovementSFX();
        hudGo.SetActive(false);
        resetGame();

        bringNewBatch();
        yield return this.Wait(1f);

        panCamera.PanCameraDown();
        while (true == panCamera.IsMovingCamera) yield return null;

        hudGo.SetActive(true);
        doraController.StartController();
        timer.SetTimer(doraGameData.BaseTimer, true);
        sfxProvider.StartMusic();
    }

    protected override void onGameplayStarted()
    {
        registerEvents();
        timer.StartTimer();
        bringNextCob();
    }

    protected override void onGameplayEnded()
    {
        unregisterEvents();
    }

    protected override IEnumerator onSuccess()
    {
        sfxProvider.StopMusic();
        doraController.StopController();

        while (true == doraController.IsEating) yield return null;

        yield return StartCoroutine(moveCurrentCob(true));

        while (true == UIkernelManager.IsDequeing) yield return null;
        while (true == scorePopupSpawner.HasLivingPopups) yield return null;

        showEndGamePopup();
    }

    protected override IEnumerator onFailure() { yield break; }
    #endregion

    #region PRIVATE

    void bringNextCob()
    {
        if (null != getNextCobRoutine) return;

        enableOffScreenCobKernels(true);

        Transform cob = unstackNextCob();
        if (null == cob)
        {
            getNextDoraBatch();
        }
        else
        {
            getNextCobRoutine = StartCoroutine(onGetNextCob(cob));
        }
    }

    void showEndGamePopup()
    {
        hudGo.SetActive(false);
        endGamePopup.SetScore(scoreManager.GetScoreString());
        endGamePopup.Appear(true);

        sfxProvider.PlayMovementSFX();

        disposeAllCoroutines();
        resetGame();
    }

    IEnumerator moveCurrentCob(bool i_offScreen)
    {
        if (null == currentCob) yield break;
        sfxProvider.PlayMovementSFX();

        if(true == i_offScreen)
        {
            yield return StartCoroutine(doraMover.MoveCobOffScreen(currentCob, cobExitTime));
            DoraCellMap cellMap = currentCob.GetComponent<DoraCellMap>();
            if (cellMap != null) doraSpawner.DespawnDoraCob(cellMap);
        }
        else
            yield return StartCoroutine(doraMover.MoveCobToGameView(currentCob, cobEnterTime));
    }

    IEnumerator onGetNextCob(Transform i_cob)
    {
        yield return StartCoroutine(moveCurrentCob(true));

        currentCob = i_cob;

        DoraDurabilityManager durability = i_cob.GetComponent<DoraDurabilityManager>();
        durability.UpdateDurability(true);

        panCamera.PanCameraDown();
        while (true == panCamera.IsMovingCamera) yield return null;
        yield return this.Wait(0.5f);

        panCamera.PanCameraUp();

        yield return StartCoroutine(moveCurrentCob(false));

        enableOffScreenCobKernels(false);

        DoraCellMap cellMap = i_cob.GetComponent<DoraCellMap>();
        AutoRotator rotator = i_cob.GetComponent<AutoRotator>();

        tryStartDoraGameplay(cellMap, rotator, durability);
    }

    void enableOffScreenCobKernels(bool i_enable)
    {
        DoraCellMap currDora = null;
        foreach (Transform dora in doraCobStack)
        {
            currDora = dora.GetComponent<DoraCellMap>();

            if (currDora != null)
                currDora.EnableRenderers(i_enable);
        }

        charcoalGroup.SetActive(i_enable);
    }

    void registerCob(Transform i_doraCob)
    {
        if (i_doraCob == null)
        {
            Debug.LogError("Transform is null! Cannot register cob");
            return;
        }

        if (doraCobStack == null)
            doraCobStack = new Stack<Transform>();

        doraCobStack.Push(i_doraCob);
    }

    Transform unstackNextCob()
    {
        Transform nextCob = null;
        if (doraCobStack != null && doraCobStack.Count > 0)
            nextCob = doraCobStack.Pop();

        return nextCob;
    }

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

        this.DisposeCoroutine(ref getNextCobRoutine);
        doraMover.ResetMover();

        if (null != doraCobStack) doraCobStack.Clear();
        currentCob = null;

        timer.ResetTimer();
        currentDurabilityManager = null;
        currentDoraBatch = null;
        doraBatchCount = 0;
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

            if(null != currCob)
            {
                registerCob(currCob.transform);

                if (true == superKernelSpawned)
                    superKernelCobsSpawned++;
            }
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

        bringNextCob();
    }

    private void goToSuccess() { EndMiniGame(true); }

    private void getNextDoraBatch() {  StartCoroutine(doraBatchSequence()); }

    void onEat()
    {
        if (doraController.DidEatAllKernels || doraController.GoodKernelsEatenCount == currentDurabilityManager.UnburntKernels)
        {
            currentDurabilityManager = null;
            doraController.DisableController();
            bringNextCob();
        }
    }

    private void tryStartDoraGameplay(DoraCellMap i_cellMap, AutoRotator i_autoRotate, DoraDurabilityManager i_durabilityManager)
    {
        doraController.SetDoraComponents(i_cellMap, i_autoRotate, i_durabilityManager.UnburntKernels);
        doraController.EnableController();
        doraController.StartAutoRotation();

        currentDurabilityManager = i_durabilityManager;
    }

    private void registerEvents()
    {
        timer.OnTimerEnded += goToSuccess;
        doraController.OnDidFinishEating += onEat;
    }

    private void unregisterEvents()
    {
        timer.OnTimerEnded -= goToSuccess;
        doraController.OnDidFinishEating -= onEat;
    }

    #endregion
}
