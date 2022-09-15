using PathologicalGames;
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
    [SerializeField] DoraGameplayData DoraGameplayData = null;
    [SerializeField] UIDoraBiteAnimation biteAnimation = null;
    [SerializeField] SpawnPool biteAnimationPool = null;
    [SerializeField] protected InterpolatorsManager interpolators = null;
    [SerializeField] UIDoraEatRangeFeedback rangeFeedback = null;


    [Header("Score")]
    [SerializeField] DoraScoreManager scoreManager = null;
    [SerializeField] UIKernelManager uiKernelManager = null;
    [SerializeField] float animationTime = 0.5f;
    [SerializeField] float animationOffset = 10f;
    [SerializeField] float alphaTime = 0.2f;

    [Header("SFX")]
    [SerializeField] private AudioSource[] bigBiteSFXs = null;
    [SerializeField] private AudioSource smallBiteSFX = null;
    [SerializeField] private AudioSource chewSFX = null;
    [SerializeField] private AudioSource[] burntKernelSFXs = null;

    private static readonly string BITE_ANIMATION_PREFAB = "UIPooledBiteAnimation";
    Coroutine eatingRoutine = null;
    protected Coroutine frenzyRoutine = null;
    AutoRotator autoRotator = null;
    int unburntEatenCount = 0;
    int selectedRadius = 0;

    private bool didGameplayEnd = false;

    protected DoraCellMap cellMap = null;

    #region UNITY AND CORE

    protected virtual void Start()
    {
        enableControllerUI(false);
        listenToInputs();
    }

    #endregion

    #region PUBLIC API

    public int CurrentSelectionRadius => null == frenzyRoutine ? selectedRadius : DoraGameplayData.FrenzySelectionRange;

    public int MaxSelectionRadius => null == cellSelector ? 1 : cellSelector.MaxSelectionRadius;

    public bool IsSelectingKernel()
    {
        if (null == cellSelector.CurrentOriginCell) return false;
        DoraCellData cell = cellMap.GetCell(cellSelector.CurrentOriginCell.Value, false, false);
        return cell.HasKernel;
    }

    public int UnburntEatenCount => unburntEatenCount;

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

        stopFrenzyMode();

        if (true == i_clearSelection)
            cellSelector.ClearSelection();
    }

    public void DisableMoveControls()
    {
        inputs.DisableMoveInputs();
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

        unburntEatenCount = 0;
    }

    public bool IsEating
    {
        get
        {
            if (null != eatingRoutine) return true;
            return false;
        }
    }

    public void StopController()
    {
        didGameplayEnd = true;

        stopFrenzyMode();

        unlistenToInputs();
        StopAutoRotation();
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
            cellSelector.SelectRange(currentSelect.Value, selectedRadius, true, false, false);
            selectedRadius++;
        }
    }

    protected virtual void onEatReleased()
    {
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
        int goodKernelsCount = enqueueEatenKernels(cellSelector.SelectedRangeInSteps, ref cellsToCleanup, out canStartFrenzyMode);
        eatingRoutine = StartCoroutine(eatingSequence(cellsToCleanup, goodKernelsCount, canStartFrenzyMode));
    }

    int enqueueEatenKernels(
        IReadOnlyList<HashSet<Vector2Int>> i_selectedKernelsInSteps, 
        ref HashSet<DoraCellData> i_cellsToCleanup,
        out bool i_startFrenzyMode)
    {
        i_startFrenzyMode = false;
        int burntKernelsCount = 0;
        int eatenKernels = 0;

        if(null == i_cellsToCleanup) i_cellsToCleanup = new HashSet<DoraCellData>();
        List<HashSet<DoraKernel>> kernelSets = new List<HashSet<DoraKernel>>();
        DoraCellData cell = null;

        foreach (HashSet<Vector2Int> cellSet in i_selectedKernelsInSteps)
        {
            HashSet<DoraKernel> newSet = new HashSet<DoraKernel>();
            foreach (Vector2Int coord in cellSet)
            {
                cell = cellMap.GetCell(coord, false, false);

                if (cell.KernelStatus == KernelStatus.Burnt && frenzyRoutine != null)
                    continue;
                if (cell.HasKernel)
                {
                    newSet.Add(cell.Kernel);
                    if (cell.KernelStatus == KernelStatus.Burnt) burntKernelsCount++;
                    eatenKernels++;
                    i_cellsToCleanup.Add(cell);

                    if (cell.KernelStatus == KernelStatus.Super)
                        i_startFrenzyMode = true;
                }
            }

            kernelSets.Add(newSet);
        }

        uiKernelManager.EnqueueKernels(getStackInfo(kernelSets));


        return eatenKernels - burntKernelsCount;
    }

    private IEnumerator playBiteAnimation()
    {

        if (CurrentSelectionRadius == 0)
        {
            playSmallBiteSFX();

        }

        if (CurrentSelectionRadius == 0 || null != frenzyRoutine)
        {
            Transform biteTr = biteAnimationPool.Spawn(BITE_ANIMATION_PREFAB);
            UIPooledBiteAnimation bite = biteTr.GetComponent<UIPooledBiteAnimation>();
            bite.Play(biteAnimationPool, interpolators, rangeFeedback);
            yield break;
        }



        playRandomSoundFromArray(bigBiteSFXs);

        while (true == biteAnimation.IsPlaying)
            yield return null;

        while (bigBiteSFXs[0].isPlaying || bigBiteSFXs[1].isPlaying)
        {
            yield return null;
        }
        chewSFX.Play();
    }


    private void playSmallBiteSFX()
    {
        float randomPitch = Random.Range(1f, 2f);
        smallBiteSFX.pitch = randomPitch;
        Debug.LogError(randomPitch);
        smallBiteSFX.Play();
    }

    private void playRandomSoundFromArray(AudioSource[] i_audioSources)
    {
        int randomSFX = Random.Range(0, bigBiteSFXs.Length);
        i_audioSources[randomSFX]?.Play();
    }

    private IEnumerator eatingSequence(HashSet<DoraCellData> i_cellsToCleanup, int i_eatenKernel,int i_burntKernels, bool i_startFrenzy)
    {
        inputs.DisableInputs();
        yield return StartCoroutine(playBiteAnimation());

        if (i_burntKernels > 0) playRandomSoundFromArray(burntKernelSFXs);

        foreach (DoraCellData cell in i_cellsToCleanup)
        {
            // cell.Eat();
            kernelSpawner.RequestKernelDespawn(cell.Kernel, false);
            cell.Reset();
        }

        unburntEatenCount += (i_eatenKernel - i_burntKernels);
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
    }

    void startFrenzy()
    {
        if (null != frenzyRoutine) return;
        frenzyRoutine = StartCoroutine(doFrenzy());
    }


    private IEnumerator doFrenzy()
    {
        Debug.LogError("Start Frenzy Mode");

        inputs.DisableMoveInputs();

        frenzyController.PlayFrenzyMode(autoRotator);

        uiKernelManager.ActivateFrenzy(true);

        yield return frenzyController.PlayFrenzyMode(autoRotator);

        inputs.EnableInputs();

        StartAutoRotation();

        stopFrenzyMode();
    }

    private void stopFrenzyMode()
    {
        frenzyController.StopFrenzyMode();
        uiKernelManager.ActivateFrenzy(false);
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
