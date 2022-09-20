using PathologicalGames;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DoraAbstractController : MonoBehaviourBase
{
    [SerializeField] protected bool useRangeMarking = false;
    [SerializeField] DoraInputs inputs = null;
    [SerializeField] protected DoraCellSelector cellSelector = null;
    [SerializeField] KernelSpawner kernelSpawner = null;
    [SerializeField] DoraFrenzyController frenzyController = null;
    [SerializeField] protected DoraGameplayData DoraGameplayData = null;
    [SerializeField] UIDoraBiteAnimation biteAnimation = null;
    [SerializeField] SpawnPool biteAnimationPool = null;
    [SerializeField] protected InterpolatorsManager interpolators = null;
    [SerializeField] UIDoraEatRangeFeedback rangeFeedback = null;
    [SerializeField] ShakeEffect2D cameraShake = null;
    [SerializeField] DoraSFXProvider sfxProvider = null;

    [Header("Score")]
    [SerializeField] UIKernelManager uiKernelManager = null;

    Coroutine eatingRoutine = null;
    protected Coroutine frenzyRoutine = null;

    AutoRotator autoRotator = null;
    protected DoraCellMap cellMap = null;

    private static readonly string BITE_ANIMATION_PREFAB = "UIPooledBiteAnimation";

    int totalEatenCount = 0;
    int goodEatenCount = 0;
    int burntEatenCount = 0;
    int selectedRadius = 0;
    bool didGameplayEnd = false;

    public Action OnDidFinishEating = null;

    #region UNITY AND CORE

    protected virtual void Start()
    {
        enableControllerUI(false);
        listenToInputs();
    }

    #endregion

    #region PUBLIC API

    [ExposePublicMethod]
    public void EatAllKernels()
    {
        onEatStarted();
        cellSelector.SelectAll();
        onEatReleased();
    }

    public int CurrentSelectionRadius => null == frenzyRoutine ? selectedRadius : DoraGameplayData.FrenzySelectionRange;

    public int MaxSelectionRadius => null == cellSelector ? 1 : cellSelector.MaxSelectionRadius;

    public bool IsSelectingKernel()
    {
        if (null == cellSelector.CurrentOriginCell) return false;
        DoraCellData cell = cellMap.GetCell(cellSelector.CurrentOriginCell.Value, false, false);
        return cell.HasKernel;
    }

    public bool DidEatAllKernels => totalEatenCount == cellMap.TotalCellCount;

    public int GoodKernelsEatenCount => goodEatenCount;

    public int TotalKernelsEatenCount => totalEatenCount;

    public int BurntKernelsEatenCount => burntEatenCount;

    [ExposePublicMethod]
    public void EnableController()
    {
        Debug.LogError("Enable Controller");
        inputs.EnableInputs();
        enableControllerUI(true);
    }

    [ExposePublicMethod]
    public void DisableController(bool i_clearSelection = true)
    {
        inputs.DisableInputs();
        enableControllerUI(false);
        StopAutoRotation();
        stopFrenzyMode();

        if (true == i_clearSelection)
            cellSelector.ClearSelection();
    }

    public virtual void StartAutoRotation(bool i_setDefaultSpeed = true)
    {
        if (true == i_setDefaultSpeed) autoRotator?.SetRotationSpeedX(DoraGameplayData.DefaultRotationSpeed);
        autoRotator?.StartAutoRotation();
    }

    public virtual void StopAutoRotation()
    {
        autoRotator?.StopAutoRotation();
    }

    public void SetDoraComponents(DoraCellMap i_cellMap, AutoRotator i_autoRotator)
    {
        autoRotator = i_autoRotator;
        cellMap = i_cellMap;
        if (null == cellMap) DisableController();
        cellSelector.SetCellMap(cellMap);

        KernelSpawner kSpawner = i_cellMap.GetComponentInChildren<KernelSpawner>();
        if (kSpawner != null)
            kernelSpawner = kSpawner;
        else
            Debug.LogError("No kernel spawner attached to the provided cell map!");

        burntEatenCount = 0;
        totalEatenCount = 0;
        goodEatenCount = 0;
    }

    public bool IsEating => null != eatingRoutine;

    public void StopController()
    {
        didGameplayEnd = true;

        stopFrenzyMode();

        unlistenToInputs();
        DisableController();
    }

    #endregion

    #region PROTECTED

    protected abstract void enableControllerUI(bool i_enable);

    protected abstract void move(Vector2 i_move);

    protected virtual void onEatStarted()
    {
        if (false == IsSelectingKernel()) return;

        StopAutoRotation();
        selectedRadius = 0;
        inputs.DisableMoveInputs();
    }

    protected virtual void onEat()
    {
        Vector2Int? currentSelect = cellSelector.CurrentOriginCell;
        if (null == currentSelect) return;

        if (selectedRadius <= cellSelector.MaxSelectionRadius)
        {
            if (selectedRadius > 0)
                sfxProvider.PlayRangeSFX(0.25f);

            cellSelector.SelectRange(currentSelect.Value, selectedRadius, true, false, false);
            selectedRadius++;
        }
    }

    protected virtual void onEatReleased()
    {
        sfxProvider.StopRangeSFX();
        eatKernels();
    }

    #endregion

    #region PRIVATE

    void listenToInputs()
    {
        inputs.OnMoveStarted += onMoveStarted;
        inputs.OnMove += onMove;
        inputs.OnMoveReleased += onMoveReleased;

        inputs.OnEatStarted += onEatStarted;
        inputs.OnEat += onEat;
        inputs.OnEatReleased += onEatReleased;
    }

    void unlistenToInputs()
    {
        inputs.OnMoveStarted -= onMoveStarted;
        inputs.OnMove -= onMove;
        inputs.OnMoveReleased -= onMoveReleased;

        inputs.OnEatStarted -= onEatStarted;
        inputs.OnEat -= onEat;
        inputs.OnEatReleased -= onEatReleased;
    }

    private void onMoveStarted(Vector2 i_move) { move(i_move); }

    private void onMove(Vector2 i_move) { move(i_move); }

    private void onMoveReleased(Vector2 i_move) { }

    private void eatKernels()
    {
        if (null != eatingRoutine) return;
        if (null == cellSelector.CurrentOriginCell || false == IsSelectingKernel())
        {
            inputs.EnableInputs();
            return;
        }

        bool canStartFrenzyMode = false;
        HashSet<DoraCellData> cellsToCleanup = null;
        int totalKernelsCount, goodKernelsCount, burntKernelsCount = 0;
        enqueueEatenKernels(cellSelector.SelectedRangeInSteps, ref cellsToCleanup, out canStartFrenzyMode, out totalKernelsCount, out goodKernelsCount, out burntKernelsCount);
        eatingRoutine = StartCoroutine(eatingSequence(cellsToCleanup, totalKernelsCount, goodKernelsCount, burntKernelsCount, canStartFrenzyMode));
    }

    void enqueueEatenKernels(
        IReadOnlyList<HashSet<Vector2Int>> i_selectedKernelsInSteps,
        ref HashSet<DoraCellData> i_cellsToCleanup,
        out bool i_startFrenzyMode,
        out int i_totalKernelsCount,
        out int i_goodKernelsCount,
        out int i_burntKernelsCount)
    {
        i_startFrenzyMode = false;
        int burntKernelsCount = 0;
        int eatenKernels = 0;

        if (null == i_cellsToCleanup) i_cellsToCleanup = new HashSet<DoraCellData>();
        List<HashSet<DoraKernel>> kernelSets = new List<HashSet<DoraKernel>>();
        DoraCellData cell = null;

        foreach (HashSet<Vector2Int> cellSet in i_selectedKernelsInSteps)
        {
            HashSet<DoraKernel> newSet = new HashSet<DoraKernel>();
            foreach (Vector2Int coord in cellSet)
            {
                cell = cellMap.GetCell(coord, false, false);

                if (cell.HasKernel)
                {
                    if (cell.KernelStatus == KernelStatus.Burnt && frenzyRoutine != null)
                    {
                        // ignore
                    }
                    else
                    {
                        newSet.Add(cell.Kernel);
                        if (cell.KernelStatus == KernelStatus.Burnt) burntKernelsCount++;
                        eatenKernels++;
                        i_cellsToCleanup.Add(cell);

                        if (cell.KernelStatus == KernelStatus.Super)
                            i_startFrenzyMode = true;
                    }
                }
            }

            kernelSets.Add(newSet);
        }

        uiKernelManager.EnqueueKernels(getStackInfo(kernelSets));

        i_totalKernelsCount = eatenKernels;
        i_goodKernelsCount = eatenKernels - burntKernelsCount;
        i_burntKernelsCount = burntKernelsCount;
    }

    private IEnumerator playBiteAnimation(bool i_negative)
    {
        if (false == i_negative && (CurrentSelectionRadius == 0 || null != frenzyRoutine))
        {
            cameraShake.SetShakeDuration(0.1f);
            cameraShake.SetIntensity(0.05f);
            cameraShake.StartShake(true);
            sfxProvider.PlaySmallBiteSFX();
            Transform biteTr = biteAnimationPool.Spawn(BITE_ANIMATION_PREFAB);
            UIPooledBiteAnimation bite = biteTr.GetComponent<UIPooledBiteAnimation>();
            bite.Play(biteAnimationPool, interpolators, rangeFeedback);

            if(true == i_negative)
            {
                while (bite.IsPlaying) yield return null;
            }

            yield break;
        }

        if(false == i_negative) 
            sfxProvider.PlayBigBiteSFX();
        else
            sfxProvider.PlaySmallBiteSFX();


        biteAnimation.Play(i_negative);
        while (true == biteAnimation.IsPlaying)
            yield return null;

        if(false == i_negative)
        {
            float tShake = (float)CurrentSelectionRadius / (float)cellSelector.MaxSelectionRadius;
            cameraShake.SetShakeDuration(Mathf.Lerp(0.1f, 0.2f, tShake));
            cameraShake.SetIntensity(Mathf.Lerp(0.05f, 0.2f, tShake));
            cameraShake.StartShake(true);
        }
    }

    private IEnumerator eatingSequence(HashSet<DoraCellData> i_cellsToCleanup, int i_totalEaten, int i_goodEaten, int i_burntEaten, bool i_startFrenzy)
    {
        inputs.DisableInputs();
        yield return StartCoroutine(playBiteAnimation(i_burntEaten != 0));

        foreach (DoraCellData cell in i_cellsToCleanup)
        {
            if(null != cell.Kernel)
            {
                cell.Kernel.GetEaten();
            }

            kernelSpawner.RequestKernelDespawn(cell.Kernel, false);
            cell.Reset();
        }

        totalEatenCount += i_totalEaten;
        goodEatenCount += i_goodEaten;
        burntEatenCount += i_burntEaten;

        selectedRadius = 0;

        onEatSequenceEnded(i_startFrenzy);
    }

    void onEatSequenceEnded(bool i_startFrenzy)
    {
        this.DisposeCoroutine(ref eatingRoutine);

        if (true == didGameplayEnd) return;

        if (true == i_startFrenzy) startFrenzy();
        inputs.EnableEatInputs();
        if (null == frenzyRoutine) inputs.EnableMoveInputs();

        StartAutoRotation(null == frenzyRoutine);
        OnDidFinishEating?.Invoke();
    }

    void startFrenzy()
    {
        Debug.Log("START FRENZY MODE");

        if (null != frenzyRoutine) return;
        frenzyRoutine = StartCoroutine(doFrenzy());
    }


    private IEnumerator doFrenzy()
    {
        Debug.LogError("Start Frenzy Mode");

        inputs.DisableMoveInputs();

        frenzyController.PlayFrenzyMode(autoRotator);

        yield return frenzyController.PlayFrenzyMode(autoRotator);

        inputs.EnableInputs();

        StartAutoRotation();

        stopFrenzyMode();
    }

    private void stopFrenzyMode()
    {
        Debug.Log("STOP FRENZY MODE");

        frenzyController.StopFrenzyMode();
        this.DisposeCoroutine(ref frenzyRoutine);
    }

    private Queue<ScoreKernelInfo> getStackInfo(List<HashSet<DoraKernel>> i_eatenKernels)
    {
        Queue<ScoreKernelInfo> scoreKernels = new Queue<ScoreKernelInfo>();
        float multiplier = 1f;
        HashSet<DoraKernel> currSet = null;
        int length = i_eatenKernels.Count;
        for (int i = 0; i < length; i++)
        {
            currSet = i_eatenKernels[i];
            foreach (DoraKernel kernel in currSet)
            {
                scoreKernels.Enqueue(new ScoreKernelInfo(multiplier, kernel.Status));
            }
            multiplier += 1;
        }

        return scoreKernels;
    }

    #endregion
}
