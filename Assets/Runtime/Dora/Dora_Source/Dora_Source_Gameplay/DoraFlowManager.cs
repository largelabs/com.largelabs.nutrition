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
    [SerializeField] private UIMinigameTimer uiMiniGameTimer = null;

    [Header("VFX / SFX")]
    [SerializeField] private InterpolatorsManager interpolators = null;
    [SerializeField] private PanCamera panCamera = null;
    [SerializeField] private SpawnPool vfxPool = null;
    [SerializeField] private DoraSFXProvider sfxProvider = null;
    [SerializeField] private ParticleSystem fireParticlesBurst = null;
    [SerializeField] private UIAppearWithCanvasGroupAlpha uiHighlightBg = null;

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
    DoraCellFactory cellFactory = null;

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

    [ExposePublicMethod]
    public void GetNextDoraBatch() { StartCoroutine(doraBatchSequence()); }

    #endregion

    #region GAME_FLOW

    protected override IEnumerator introRoutine()
    {
        sfxProvider.PlayMovementSFX();
        hudGo.SetActive(false);
        resetGame();

        yield return StartCoroutine(bringNewBatch());
        yield return this.Wait(1f);
        updateStackDurability();

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
            GetNextDoraBatch();
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

    void despawnCob(Transform i_cob)
    {
        if (null == i_cob) return;

        DoraCellMap cellMap = i_cob.GetComponent<DoraCellMap>();
        if (cellMap != null) doraSpawner.DespawnDoraCob(cellFactory, cellMap);
    }

    IEnumerator moveCurrentCob(bool i_offScreen)
    {
        if (null == currentCob) yield break;
        sfxProvider.PlayMovementSFX();

        Transform cob = currentCob;

        if(true == i_offScreen)
        {
            currentCob = null;
            yield return StartCoroutine(doraMover.MoveCobOffScreen(cob, cobExitTime));
            despawnCob(cob);
        }
        else
            yield return StartCoroutine(doraMover.MoveCobToGameView(cob, cobEnterTime));
    }

    void updateStackDurability()
    {
        if (null == doraCobStack) return;

        foreach(Transform cob in doraCobStack)
        {
            DoraDurabilityManager durability = cob.GetComponent<DoraDurabilityManager>();
            durability.UpdateDurability(true);
        }

    }

    IEnumerator onGetNextCob(Transform i_cob)
    {
        yield return StartCoroutine(moveCurrentCob(true));

        currentCob = i_cob;
        updateStackDurability();

        panCamera.PanCameraDown();
        while (true == panCamera.IsMovingCamera) yield return null;
        yield return this.Wait(0.5f);

        panCamera.PanCameraUp();

        yield return StartCoroutine(moveCurrentCob(false));

        enableOffScreenCobKernels(false);

        DoraDurabilityManager durability = i_cob.GetComponent<DoraDurabilityManager>();
        DoraCellMap cellMap = i_cob.GetComponent<DoraCellMap>();
        AutoRotator rotator = i_cob.GetComponent<AutoRotator>();

        tryStartDoraGameplay(cellMap, rotator, durability);

        this.DisposeCoroutine(ref getNextCobRoutine);
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
        if (null == cellFactory) cellFactory = new DoraCellFactory(interpolators);

        unregisterEvents();

        doraSpawner.DespawnAllDora(cellFactory);
        doraController.DisableController();

        scoreManager.ResetScoreManager();

        this.DisposeCoroutine(ref getNextCobRoutine);
        doraMover.ResetMover();

        clearCobStack();
        currentCob = null;

        timer.ResetTimer();
        currentDurabilityManager = null;
        currentDoraBatch = null;
        doraBatchCount = 0;
    }

    void clearCobStack()
    {
        if (null == doraCobStack) return;

        foreach (Transform dora in doraCobStack)
            despawnCob(dora);

        doraCobStack.Clear();
    }


    IEnumerator bringNewBatch()
    {
        yield return StartCoroutine(moveCurrentCob(true));
        clearCobStack();

        // Maybe change way of choosing batch?
        currentDoraBatch = doraBatchData[UnityEngine.Random.Range(0, doraBatchData.Count)];

        doraBatchCount++;

        DoraCellMap currCob = null;
        int length = Mathf.Clamp(currentDoraBatch.DoraInBatch, 1, 4);
        bool superKernelSpawned;
        int superKernelCobsSpawned = 0;

        float superKernelChance = currentDoraBatch.SuperKernelChance;

        Transform anchor = null;

        for (int i = 0; i < length; i++)
        {
            anchor = doraPlacer.GetNextAnchor();
            currCob = doraPlacer.SpawnDoraAtAnchor(anchor, MathConstants.VECTOR_3_UP * 10f);
            currCob.InitializeDoraCob(doraController, cellFactory, vfxPool, cullingBounds, selectionBounds, currentDoraBatch, doraBatchCount, canSpawnSuper(superKernelCobsSpawned, ref superKernelChance), out superKernelSpawned);

            if(null != currCob)
            {
                registerCob(currCob.transform);

                if (true == superKernelSpawned)
                    superKernelCobsSpawned++;

                currCob.transform.localRotation = Quaternion.Euler(new Vector3(Random.Range(0f, 20f), Random.Range(0f, 20f), Random.Range(0f, 20f)));

                doraMover.MoveCob(currCob.transform, anchor, 1f, onCobLanded);
                yield return this.Wait(0.5f);
            }
        }
    }

    void onCobLanded(Transform i_cob)
    {
        i_cob.localRotation = MathConstants.QUATERNION_IDENTITY;
        sfxProvider.PlayLandedSFX();
        fireParticlesBurst.Play();
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
        doraController.DisableController();

        yield return StartCoroutine(moveCurrentCob(true));

        timer.PauseTimer();

        uiHighlightBg.Init(interpolators);
        uiHighlightBg.Appear(true);
        yield return this.Wait(0.5f);

      //  scoreManager.AddScoreByValue(currentDoraBatch.BatchFinishScoreBonus, 
       //     PopupSpawner.PopupType.Super, Vector3.zero, 1.0f, 0.5f, 10f);

     //   yield return this.Wait(1.0f);

        uiMiniGameTimer.MoveTimerRootToCenter();
        yield return this.Wait(0.5f);

        uiMiniGameTimer.PlayTimeBonusPopup(currentDoraBatch.BatchFinishTimeBonus);

        yield return this.Wait(0.3f);

        uiMiniGameTimer.BumpTimer();
        timer.AddTime(currentDoraBatch.BatchFinishTimeBonus);
        sfxProvider.PlayTimeBonusSFX();

        yield return this.Wait(0.5f);

        uiMiniGameTimer.MoveTimerRootToInitialPosition();

        uiHighlightBg.Disappear(true);
        yield return this.Wait(1.0f);

        yield return StartCoroutine(bringNewBatch());
        yield return this.Wait(1.0f);
        updateStackDurability();

        timer.ResumeTimer();

        bringNextCob();
    }

    private void goToSuccess() { EndMiniGame(true); }

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
