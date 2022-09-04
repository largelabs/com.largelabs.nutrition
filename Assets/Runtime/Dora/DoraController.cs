using System.Collections.Generic;
using UnityEngine;

public class DoraController : MonoBehaviourBase
{
    [SerializeField] bool forceSelectionOnEat = true;
    [SerializeField] DoraInputs inputs = null;
    [SerializeField] protected DoraAbstractCellSelector cellSelector = null;
    [SerializeField] KernelSpawner kernelSpawner = null;
    [SerializeField] DoraScoreManager scoreManager = null;

    protected DoraCellMap cellMap = null;

    private int currentEatenKernelCount = 0;

    #region UNITY AND CORE

    void Start ()
    {
        listenToInputs();
    }

    #endregion

    #region PUBLIC API

    public IRangeSelectionProvider SelectionProvider => cellSelector;

    public IDoraCellProvider CurrentCellProvider => cellMap;

    public int CurrentEatenKernelCount => currentEatenKernelCount;

    [ExposePublicMethod]
    public void EnableController()
    {
        inputs.EnableInputs();
    }

    public virtual void StartAutoRotation()
    {
        cellSelector.StartAutoRotation();
    }

    public virtual void StopAutoRotation()
    {
        cellSelector.StopAutoRotation();
    }

    [ExposePublicMethod]
    public void DisableController(bool i_clearSelection = true)
    {
        inputs.DisableInputs();

        if (true == i_clearSelection)
            cellSelector.ClearSelection();
    }

    public void SetCellMap(DoraCellMap i_cellMap)
    {
        cellMap = i_cellMap;
        if (null == cellMap) DisableController();
        cellSelector.SetCellMap(cellMap);

        KernelSpawner kSpawner = i_cellMap.GetComponentInChildren<KernelSpawner>();
        if (kSpawner != null)
            kernelSpawner = kSpawner;
        else
            Debug.LogError("No kernel spawner attached to the provided cell map!");

        currentEatenKernelCount = 0;
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

    int selectedRadius = 0;

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
            cellSelector.SelectRange(currentSelect.Value, selectedRadius, true, false, false);
            selectedRadius++;
        }
    }

    private void onEatReleased()
    {
        eatKernels();

        if(null != cellSelector.CurrentOriginCell && true == forceSelectionOnEat)
            cellSelector.SelectCell(cellSelector.CurrentOriginCell.Value, false, true);

        StartAutoRotation();

        selectedRadius = 0;
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
        IReadOnlyList<Vector2Int> selectedCells = cellSelector.SelectedRange;
        if (null == selectedCells) return;

        int burntKenrelsCount = 0;
        int eatenKernels = 0;

        int count = selectedCells.Count;

        for (int i = 0 ; i < count; i++)
        {
            DoraCellData cell = cellMap.GetCell(selectedCells[i], false, false);

            if (cell.HasKernel)
            {
                if (true == cell.KernelIsBurnt()) burntKenrelsCount++;
                kernelSpawner.DespawnKernel(cell.Kernel);
                cell.Reset();
                eatenKernels++;
            }
        }

        Debug.LogError(eatenKernels);

        scoreManager.AddScore(eatenKernels - burntKenrelsCount);
        scoreManager.RemoveScore(burntKenrelsCount);

        currentEatenKernelCount += eatenKernels;
    }

    #endregion
}
