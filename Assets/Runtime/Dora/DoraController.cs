using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoraController : MonoBehaviourBase
{
    [SerializeField] DoraCellMap defaultCellMap = null;
    [SerializeField] DoraCellSelector cellSelector = null;
    [SerializeField] KernelSpawner kernelSpawner = null;
    [SerializeField] DoraScoreManager scoreManager = null;

    DoraActions inputActions = null;
    DoraCellMap cellMap = null;
    Coroutine moveRoutine = null;
    Coroutine eatRoutine = null;

    private int currentEatenKernelCount = 0;

    #region UNITY AND CORE

    void Start ()
    {
        initInputs();
        EnableController();

        if(defaultCellMap != null)
            SetCellMap(defaultCellMap);
    }

    #endregion

    #region PUBLIC API

    public IDoraCellProvider CurrentCellProvider => cellMap;

    public int CurrentEatenKernelCount => currentEatenKernelCount;

    [ExposePublicMethod]
    public void EnableController()
    {
        inputActions.Player.Move.Enable();
    }

    [ExposePublicMethod]
    public void DisableController()
    {
        this.DisposeCoroutine(ref moveRoutine);
        inputActions.Player.Move.Disable();
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

    IEnumerator dispatchEatRoutine()
    {
        while (true)
        {
            Vector2Int? currentSelect = cellSelector.CurrentOriginCell;
            if (null == currentSelect)
            {
                this.DisposeCoroutine(ref eatRoutine);
                yield break;
            }

            int selectRadius = 0;

            while (true == inputActions.Player.Eat.IsPressed())
            {
                if(selectRadius <= cellSelector.MaxSelectionRadius)
                {
                    cellSelector.SelectRange(cellSelector.CurrentOriginCell.Value, selectRadius, true, false, false);
                    yield return this.Wait(0.25f);
                    selectRadius++;
                }
                else
                {
                    yield return null;
                }

            }

            yield return null;
        }
    }

    IEnumerator dispatchMoveRoutine()
    {
        while (true)
        {
            Vector2Int? currentSelect = cellSelector.CurrentOriginCell;
            if (null == currentSelect)
            {
                this.DisposeCoroutine(ref moveRoutine);
                yield break;
            }

            Vector2 inputValue = inputActions.Player.Move.ReadValue<Vector2>();

            Vector2Int nextSelect = currentSelect.Value;
            nextSelect.y += (int)inputValue.x;
            nextSelect.x += (int)inputValue.y;

            cellSelector.SelectCell(nextSelect, true, true);

            yield return this.Wait(0.2f);
        }
    }

    void initInputs()
    {
        if (null == inputActions) inputActions = new DoraActions();
        inputActions.Player.Move.started += onMoveStarted;
        inputActions.Player.Move.canceled += onMoveCanceled;

        inputActions.Player.Eat.started += onEatStarted;
        inputActions.Player.Eat.canceled += onEatCanceled;
    }

    private void onEatStarted(InputAction.CallbackContext obj)
    {
        if (null != eatRoutine) return;
        inputActions.Player.Move.Disable();

        Debug.Log("pressed");

        eatRoutine = StartCoroutine(dispatchEatRoutine());
    }

    private void onEatCanceled(InputAction.CallbackContext obj)
    {
        inputActions.Player.Move.Enable();

        Debug.Log("released");

        eatKernels();

        this.DisposeCoroutine(ref eatRoutine);
    } 

    private void onMoveStarted(InputAction.CallbackContext obj)
    {
        if (null != moveRoutine) return;
        inputActions.Player.Eat.Disable();
        moveRoutine = StartCoroutine(dispatchMoveRoutine());
    }

    private void onMoveCanceled(InputAction.CallbackContext obj)
    {
        inputActions.Player.Eat.Enable();
        inputActions.Player.TestAction.Enable();
        this.DisposeCoroutine(ref moveRoutine);
    }

    private void eatKernels()
    {
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

        scoreManager.AddScore(eatenKernels - burntKenrelsCount);
        scoreManager.RemoveScore(burntKenrelsCount);

        currentEatenKernelCount += eatenKernels;
    }

    #endregion


}
