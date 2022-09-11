using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoraController : MonoBehaviourBase
{
    [SerializeField] protected bool useRangeMarking = false;
    [SerializeField] bool forceSelectionOnEat = true;
    [SerializeField] DoraInputs inputs = null;
    [SerializeField] protected DoraCellSelector cellSelector = null;
    [SerializeField] KernelSpawner kernelSpawner = null;

    [Header("Score")]
    [SerializeField] DoraScoreManager scoreManager = null;
    [SerializeField] UIKernelManager uiKernelManager = null;
    [SerializeField] float animationTime = 0.5f;
    [SerializeField] float animationOffset = 10f;
    [SerializeField] float alphaTime = 0.2f;

    AutoRotator autoRotator = null;
    int unburntEatenCount = 0;
    int selectedRadius = 0;
    Coroutine eatingRoutine = null;

    protected DoraCellMap cellMap = null;


    #region UNITY AND CORE

    void Start ()
    {
        listenToInputs();
    }

    #endregion

    #region PUBLIC API

    public int CurrentSelectionRadius => selectedRadius;

    public IRangeSelectionProvider SelectionProvider => cellSelector;

    public IDoraCellProvider CurrentCellProvider => cellMap;

    public int UnburntEatenCount => unburntEatenCount;

    [ExposePublicMethod]
    public void EnableController()
    {
        inputs.EnableInputs();
    }

    public virtual void StartAutoRotation()
    {
        autoRotator?.StartAutoRotation();
    }

    public virtual void StopAutoRotation()
    {
        autoRotator?.StopAutoRotation();
    }

    [ExposePublicMethod]
    public void DisableController(bool i_clearSelection = true)
    {
        inputs.DisableInputs();

        if (true == i_clearSelection)
            cellSelector.ClearSelection();
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

    #endregion

    #region PRIVATE

    protected virtual void move(Vector2 i_move)
    {
        Vector2Int? currentSelect = cellSelector.CurrentOriginCell;
        if (null == currentSelect) return;

        Vector2Int nextSelect = currentSelect.Value;
        nextSelect.y += (int)i_move.x;

        cellSelector.SelectCell(nextSelect, true, true);
    }

    void listenToInputs()
    {
        inputs.OnMoveStarted += onMoveStarted;
        inputs.OnMove += onMove;
        inputs.OnMoveReleased += onMoveReleased;

        inputs.OnEatStarted += onEatStarted;
        inputs.OnEat += onEat;
        inputs.OnEatReleased += onEatReleased;
    }

    private void onEatStarted()
    {
        if (null == cellSelector.CurrentOriginCell) return;
        DoraCellData cell = cellMap.GetCell(cellSelector.CurrentOriginCell.Value, false, false);
        if (false == cell.HasKernel) return;

        StopAutoRotation();
        selectedRadius = 0;

    }

    private void onEat()
    {
        Vector2Int? currentSelect = cellSelector.CurrentOriginCell;
        if (null == currentSelect) return;

        if (selectedRadius <= cellSelector.MaxSelectionRadius)
        {
            /*if(useRangeMarking && selectedRadius < cellSelector.MaxSelectionRadius)
            {
                cellSelector.MarkRange(currentSelect.Value, selectedRadius + 1, true, false, false);
            }*/

            cellSelector.SelectRange(currentSelect.Value, selectedRadius, true, false, false);
            selectedRadius++;
        }
    }

    private void onEatReleased()
    {
        eatKernels();

        //if(null != cellSelector.CurrentOriginCell && true == forceSelectionOnEat)
        //    cellSelector.SelectCell(cellSelector.CurrentOriginCell.Value, false, true);

        //StartAutoRotation();

        //selectedRadius = 0;
    } 

    private void onMoveStarted(Vector2 i_move)
    {
        move(i_move);
    }

    private void onMove(Vector2 i_move)
    {
        move(i_move);
    }

    private void onMoveReleased(Vector2 i_move)
    {
    }

    private void eatKernels()
    {
        if (null == cellSelector.CurrentOriginCell) return;
        Debug.LogError("Eating");

        //IReadOnlyList<Vector2Int> selectedCells = cellSelector.SelectedRange;
        //if (null == selectedCells)
        //{
        //    Debug.LogError("Invalid selected cells list! Returning...");
        //    return;
        //}

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
        foreach (HashSet<Vector2Int> cellSet in selectedKernelsInSteps)
        {
            HashSet<DoraKernel> newSet = new HashSet<DoraKernel>();
            foreach (Vector2Int coord in cellSet)
            {
                DoraCellData cell = cellMap.GetCell(coord, false, false);

                if (cell.HasKernel)
                {
                    newSet.Add(cell.Kernel);
                    if (true == cell.KernelIsBurnt()) burntKenrelsCount++;
                    eatenKernels++;
                    cellsToCleanup.Add(cell);
                }
            }
            kernelSets.Add(newSet);
        }

        //scoreManager.AddScoreByKernels(kernelSets, animationTime, alphaTime, animationOffset);

        if (eatingRoutine == null)
            eatingRoutine = StartCoroutine(eatingSequence(cellsToCleanup, eatenKernels - burntKenrelsCount, kernelSets));
    }

    private IEnumerator eatingSequence(HashSet<DoraCellData> i_cellsToCleanup, int i_eatCount, List<HashSet<DoraKernel>> i_eatenKernels)
    {
        // bite animation stuff

        yield return null;

        uiKernelManager.HandleKernelStack(getStackInfo(i_eatenKernels));

        // cell cleanup
        foreach (DoraCellData cell in i_cellsToCleanup)
        {
            kernelSpawner.DespawnKernel(cell.Kernel);
            cell.Reset();
        }

        //Debug.LogError("going to wait: " + i_cellsToCleanup.Count * uiKernelManager.TimePerUIKernel);
        yield return this.Wait(i_cellsToCleanup.Count * uiKernelManager.TimePerUIKernel);

        // post-sequence cleanup
        unburntEatenCount += i_eatCount;

        if (null != cellSelector.CurrentOriginCell && true == forceSelectionOnEat)
            cellSelector.SelectCell(cellSelector.CurrentOriginCell.Value, false, true);

        StartAutoRotation();

        selectedRadius = 0;

        this.DisposeCoroutine(ref eatingRoutine);
    }

    private List<ScoreKernelInfo> getStackInfo(List<HashSet<DoraKernel>> i_eatenKernels)
    {
        List<ScoreKernelInfo> scoreKernels = new List<ScoreKernelInfo>();
        float multiplier = 1f;
        HashSet<DoraKernel> currSet = null;
        int length = i_eatenKernels.Count;
        for (int i = 0; i < length; i++)
        {
            currSet = i_eatenKernels[i];
            foreach (DoraKernel kernel in currSet)
            {
                scoreKernels.Add(new ScoreKernelInfo(multiplier, kernel.Status));
            }
            multiplier += 1;
        }

        return scoreKernels;
    }
    #endregion
}
