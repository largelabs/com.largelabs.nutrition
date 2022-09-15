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

    [Header("Score")]
    [SerializeField] DoraScoreManager scoreManager = null;
    [SerializeField] UIKernelManager uiKernelManager = null;
    [SerializeField] float animationTime = 0.5f;
    [SerializeField] float animationOffset = 10f;
    [SerializeField] float alphaTime = 0.2f;

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

    public int CurrentSelectionRadius => selectedRadius;

    public int MaxSelectionRadius => null == cellSelector ? 1 : cellSelector.MaxSelectionRadius;

    public IRangeSelectionProvider SelectionProvider => cellSelector;

    public IDoraCellProvider CurrentCellProvider => cellMap;

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

    public void EnableMoveControls()
    {
        inputs.EnableMoveInputs();
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

    public bool IsEating()
    {
        if (null == eatingRoutine) return false;
        else return true;
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

        if (frenzyRoutine == null) StopAutoRotation();
        selectedRadius = 0;
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
        if (null == cellSelector.CurrentOriginCell) return;

        DoraCellData cell = cellMap.GetCell(cellSelector.CurrentOriginCell.Value, false, false);
        if (false == cell.HasKernel) return;

        //Debug.LogError("Eating");

        IReadOnlyList<HashSet<Vector2Int>> selectedKernelsInSteps = cellSelector.SelectedRangeInSteps;
        if (null == selectedKernelsInSteps)
        {
            Debug.LogError("Invalid selected cell steps list! Returning...");
            return;
        }

        int burntKenrelsCount = 0;
        int eatenKernels = 0;

        List<HashSet<DoraKernel>> kernelSets = new List<HashSet<DoraKernel>>();
        HashSet<DoraCellData> cellsToCleanup = new HashSet<DoraCellData>();

        bool startFrenzyMode = false;

        foreach (HashSet<Vector2Int> cellSet in selectedKernelsInSteps)
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
                    if (cell.KernelStatus == KernelStatus.Burnt) burntKenrelsCount++;
                    eatenKernels++;
                    cellsToCleanup.Add(cell);

                    if (cell.KernelStatus == KernelStatus.Super)
                    {
                        startFrenzyMode = true;
                    }
                }
            }
            kernelSets.Add(newSet);
        }

        uiKernelManager.EnqueueKernels(getStackInfo(kernelSets));

        eatingRoutine = StartCoroutine(eatingSequence(cellsToCleanup, eatenKernels - burntKenrelsCount, startFrenzyMode));
    }

    private IEnumerator eatingSequence(HashSet<DoraCellData> i_cellsToCleanup, int i_eatCount, bool i_startFrenzy)
    {
        biteAnimation.Play();

        if (frenzyRoutine == null)
        {
            yield return null;

            while (true == biteAnimation.IsPlaying)
            {
                yield return null;
            }
        }

        // cell cleanup
        foreach (DoraCellData cell in i_cellsToCleanup)
        {
            kernelSpawner.RequestKernelDespawn(cell.Kernel, false);
            cell.Reset();
        }

        //Debug.LogError("going to wait: " + i_cellsToCleanup.Count * uiKernelManager.TimePerUIKernel);

        // if(frenzyRoutine == null)
        //     yield return this.Wait(i_cellsToCleanup.Count * uiKernelManager.TimePerUIKernel);

        // post-sequence cleanup
        unburntEatenCount += i_eatCount;

        if (false == didGameplayEnd && null == frenzyRoutine)
            StartAutoRotation();

        EnableMoveControls();

        selectedRadius = 0;

        if (frenzyRoutine == null && true == i_startFrenzy)
            frenzyRoutine = StartCoroutine(startFrenzyMode());

        this.DisposeCoroutine(ref eatingRoutine);
    }


    private IEnumerator startFrenzyMode()
    {
        Debug.LogError("Start Frenzy Mode");

        DisableMoveControls();

        frenzyController.PlayFrenzyMode(autoRotator);

        uiKernelManager.ActivateFrenzy(true);

        yield return frenzyController.PlayFrenzyMode(autoRotator);

        EnableMoveControls();

        StartAutoRotation();

        stopFrenzyMode();
    }

    private void stopFrenzyMode()
    {
        this.DisposeCoroutine(ref frenzyRoutine);
        frenzyController.StopFrenzyMode();

        uiKernelManager.ActivateFrenzy(false);
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
